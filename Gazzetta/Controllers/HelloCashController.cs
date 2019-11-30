/* I lost the original HelloCashController.cs file and this is an old-ish backup
    which i hope is a serviceable substitute to the real file
    While this lacks the finer details and is quite messy, it should give you a good idea as to
    how to integrate with HelloCash.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Gazzetta.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gazzetta.Controllers
{
    public class HelloCashController : Controller   // Probably should inherit from an API Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> manager;


        //Contact HelloCash to get these credentials 
        private const string Principal = "yourPrincipalHere";
        private const string Credentials = "superSecretPasswordHere";
        private const string System = "lucy";   // the name of the sandbox environment is actually called lucy

        private string _accessToken; /* you get this string after authentication( see GetToken() method ) with HelloCash,
                                     which will have to be passed with each request to their server.
                                     It expires after 24 hours; so you have to request another one each day
                                     unless you generate a Refresh Token(it doesn't expire, ever) from their portal,
                                     then you can pass this Refresh token instead of making requests every 24 hours.

                     >>>>>>>>>>>>>   I actually recommend the Refresh Token option, but i cant be bothered to do that 
                                     at the moment  *Shrugs* 

                     >>>>>>>>>>>>>   Update: At the time of writing this, the Refresh Token option doesn't work in the Lucy sanbox environment
                                     */


        public HelloCashController()
        {
            _context=new ApplicationDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));

        }

        //Get an access_token
		// If you go with this approach save the token to file at the end 
		// encrypt it so as to protect it from prying eyes(Look into Data Protection API (DP-API) on windows)
        public void GetToken()
        {
            //Authenticating to the endpoint which is the url: https://api-et.hellocash.net/authenticate
            var reequest = (HttpWebRequest) WebRequest.Create("https://api-et.hellocash.net/authenticate");
            reequest.ContentType = "application/json";
            reequest.Method = "POST";
            using (var streamwriter = new StreamWriter(reequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    principal = Principal,
                    credentials = Credentials,
                    system = System
                });
                streamwriter.Write(json);
            }

            var reesponse = (HttpWebResponse) reequest.GetResponse();
            using (var streamReader = new StreamReader(reesponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                dynamic d = JObject.Parse(result);
                _accessToken = d.token;
                
            }
        }


		[Authorize]
        public async Task<RedirectToRouteResult> Invoice( double price, int? book,string id)
        {
            GetToken(); // Before you request a new token, check to see if the last modified date
						// on the token file saved from above is not today if it is, no need to
						//request a new token. if last modified is not today call GetToken();
            string token = _accessToken;
            using (var client = new HttpClient())
            {
                string url = "https://api-et.hellocash.net/invoices";
                // SETTING THE AUTH HEADER TO THE TOKEN RECIEVED FROM /AUTHORIZE WITH CREDENTIALS
                client.DefaultRequestHeaders.Add("Authorization","Bearer " + token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var user = await manager.FindByIdAsync(User.Identity.GetUserId());
                var phoneNumber = user.PhoneNumber;
                //var desc = book.ToString(); 
                
                var desc = book ==  null ? id : book.ToString();
                
                var anon = new
                {
                    amount = price,
                    from = phoneNumber,
                    description = desc,
                    notifyfrom = true,
                    notifyto = true
                };

                // Serialize the anonymous type into json obj
                var jason = JsonConvert.SerializeObject(anon);
                //Wrap the json obj inside a StringContent to be used by the HttpClient class
                var httpcontent = new StringContent(jason,Encoding.UTF8,"application/json");
                //await the response
                var response = await client.PostAsync(url, httpcontent);
                if (book!=null && id == null&& response.IsSuccessStatusCode)
                {
                    var bookInDb = _context.Books.SingleOrDefault(x => x.Id == book);
                    if (bookInDb != null )
                    {
                        var theBook = bookInDb;
                        var today = DateTime.Today;
                        var userBook = new UserBook
                        {
                            ApplicationUser = user,
                            Book = theBook,
                            Status="Initialized",
                            PurchaseDate = today.Date
                            //PurchaseDate = Convert.ToDateTime(DateTime.Today.ToString("yyyy-MM-dd")) 

                        };

                        _context.UserBooks.Add(userBook);
                        _context.SaveChanges();
                    }
                    return RedirectToAction("Index", "Manage");

                }

                if (id != null && book == null&& response.IsSuccessStatusCode)
                {
                    var magInDb = _context.Magazines.SingleOrDefault(m => m.Id.ToString() == id);
                    if (magInDb != null )
                    {
                        var theMag = magInDb;
                        var today = DateTime.Today;
                        var userMagazine = new UserMagazine
                        {
                            ApplicationUser = user,
                            Magazine = theMag,
                            Status = "Initialized",
                            PurchaseDate = today.Date
                            //PurchaseDate = Convert.ToDateTime(DateTime.Today.ToString("yyyy-MM-dd")) 
                        };
                        _context.UserMagazines.Add(userMagazine);
                        _context.SaveChanges();
                        return RedirectToAction("Index", "Manage");

                    }
                }
                return RedirectToAction("Index", "Manage");



             
                //var details = response.Content.ReadAsStringAsync().Result;

//                if (response.IsSuccessStatusCode)
//                {
//                    //listen to payment processed and allow download
//                }
            }

        }


        /*  HelloCash Notifies me of payment statuses through this method here below
            I have registered this method in my Webhooks setting in my HelloCash account.
            Since I have not deployed this project, i have used a Visual Studio plugin called Conveyor-by Keyoti
		    (Check it out it is pretty awesome and necessary to relay messages from HelloCash to your local pc without deplo
            -ying this project to Azure or something 
		*/ 
         
        public ActionResult Post()
        {
            // Process data received from HelloCash
            try
            {
                // Grab the body and parse to a JSON object
                string rawBody = GetDocumentContents(global::System.Web.HttpContext.Current.Request);
                if (string.IsNullOrEmpty(rawBody))
                {
                    // No body, bad request.
                    return  new HttpStatusCodeResult(HttpStatusCode.BadRequest,"Bad request - No JSON body found!");
                }

//              >>>>>>>>>>>>>>>>    IMPORTANT COMMENTS BELOW     <<<<<<<<<<<<<<<<<<<<<<<<<<<<
//              Check here to see if the request did come from HelloCash and not from a hacker who knows 
//              your Webhook url(how to reach this method) See Comments at the end of this try block on how to do that
                    
                // for now we just assume the request is genuine
                dynamic eventObj = JsonConvert.DeserializeObject(rawBody);
                //System.IO.File.WriteAllText(@"C:\Users\UserName\Desktop\Logs\Response.txt", (string)eventObj.fromname+(string)eventObj.description+(string)eventObj.status);
                
				// the following if block is a lazy implementation but basically just record the purchase status in your Database
				
                if ((string)eventObj.status == "PROCESSED")
                {
                    
                    var description = (string) eventObj.description;
                    string phoneNo = (string) eventObj.from;

                    if (description.Length > 20)	//because guid is like 32 bytes long
                    {
                        RecordMagazinePurchase(description,phoneNo);
                        return new HttpStatusCodeResult(HttpStatusCode.OK);

                    }

                    int bookid = Int32.Parse(description);
                    RecordBookPurchase(bookid,phoneNo);
                    return new HttpStatusCodeResult(HttpStatusCode.OK);


                }

                

                return new HttpStatusCodeResult(HttpStatusCode.OK);


                // We have a request body so lets look at what we have

                // First lets ensure it hasn't been tampered with and it came from HelloCash
                // We do this by checking the HMAC from the X-Api-Hmac header
/*
                string hmac = Request.Headers["X-Api-Hmac"];
                if (String.IsNullOrEmpty(hmac))
                {
                    // No HMAC, invalid request.
                    return Unauthorized();
                }
                else
                {
                    // Validate the HMAC, ensure you has exposed the rawBody
                    // YOUR SECRET is a string generated only once from HelloCash portal when you generate a Refersh Token

                    var hash = CreateHMAC(rawBody, ">>>YOUR SECRET<<<");

                    if (hmac != hash)
                    {
                        // The request is not from HelloCash or has been tampered with
                        // Possible someone is trying to hack you
                        // return bad request or simple ok
                        return Unauthorized();
                    }

                    Continue here processing the request
                }*/

                
            }
            catch (Exception err)
            {
                var msg = "An error occurred receiving data, the error was: " + err.ToString();
               // System.IO.File.WriteAllText(@"C:\Users\UserName\Desktop\Logs\ErrorResponse.txt", msg);
                return new HttpStatusCodeResult(HttpStatusCode.OK);// Cheating here... Too many fucking Api errors, man.
//                throw;
            }
        }

        private void RecordMagazinePurchase(string description, string phoneNo)
        {
            throw new NotImplementedException();
        }

        private void RecordBookPurchase(int bookid, string phoneNo)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Retrieves the body of a HTTP request as a string
        /// </summary>
        /// <param name="Request">The HTTP Request</param>
        /// <returns>The body data as a string</returns>
        private string GetDocumentContents(HttpRequest Request)
        {
            string documentContents;
            using (Stream receiveStream = Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }
            return documentContents;
        }

        /// <summary>
        /// Should be done in a separate project with access to the DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="phoneNo"></param>
        public void RecordPurchase(int id, string phoneNo)
        {
            var userBook = _context.UserBooks
                .SingleOrDefault(ub => ub.ApplicationUser.PhoneNumber == phoneNo && ub.Book.Id == id && ub.Status == "Initialized");

            if (userBook != null)
            {
                userBook.Status = "PROCESSED";
            }
            
            _context.SaveChanges();



            

        }


    }
  


}