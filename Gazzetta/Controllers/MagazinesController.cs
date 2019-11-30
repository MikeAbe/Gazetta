/*
  A couple libraries you need to manipulate the uploaded book [to get the cover page image, get the byte size...]:

    GhostScriptSharp A dotnet library that manipulate in memory pdf obj (the book in this case)
    VersionOneEpub   A .Net library that manipulate in memory epub obj (the book in this case)
    ByteSizeLib      A .Net library to calculate, convert between byte/megabyte...
    PagedList        A .Net library for easy pagination

 */


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ByteSizeLib;
using FluentValidation.Results;
using Gazzetta.FluentValidation;
using Gazzetta.Models;
using Ghostscript.NET.Rasterizer;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using VersOne.Epub;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Gazzetta.Controllers
{
    public class MagazinesController : Controller
    {
       
  
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> manager;

        public MagazinesController()
        {
            _context = new ApplicationDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));

        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();

        }
        
        // GET: Magazines
        public ActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            var magazines = _context.Magazines.ToList();
            var onePageOfMagazines = magazines.ToPagedList(pageNumber, 10);
            ViewBag.onePageOfMagazines = onePageOfMagazines;

            return View("SearchPage",magazines);
        }
        [Authorize]
        [Authorize(Roles = RoleName.CanDoAnything)]
        public ActionResult AllMagazines()
        {
            var books = _context.Magazines.ToList();
            return PartialView("AllMagazines_Partial",books);
            
        }
        public ActionResult GetThumbnail(string id)
        {
           // var magazine = _context.Magazines.Find(id);
            var magazine = _context.Magazines.FirstOrDefault(m=>m.Id.ToString() == id);
            if (magazine != null)
            {
                return File(magazine.Publication.Thumbnail, "png");
            }

            return HttpNotFound("oops, no cover found");
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        //Asynchronous method
        public async Task<ActionResult> SaveMagazine(Magazine magazine, HttpPostedFileBase upload)
        {            
            try
            {
                //Removed b/c Id is not passed from the user form and results ModelState error if null
                ModelState.Remove("Id");
//                ModelState.Remove("Publication.Content");

                MagazineValidation magazineValidation = new MagazineValidation();
                ValidationResult val = magazineValidation.Validate(magazine);
                if (!val.IsValid)    //!ModelState.IsValid
                {
                    foreach (ValidationFailure valFailure in val.Errors)
                    {
                        ModelState.AddModelError(valFailure.PropertyName, valFailure.ErrorMessage);
                    }

                    return View("Publish", magazine);
                }
                
                //Save a new Book
               if (ModelState.IsValid )
               {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        var publication = new Publication
                        {
                            Description = magazine.Publication.Description,
                            FileType = FileType.Magazine,
                            Language = magazine.Publication.Language,
                            Name = magazine.Publication.Name,
                            Price = magazine.Publication.Price,
                            Publisher = magazine.Publication.Publisher,
                            Category = GetCategoryName(magazine.MagazineCategory),
                            Tags = magazine.Publication.Tags,
                            MediaType = upload.ContentType,
                            
                        };
                        if (upload.ContentType == @"application/epub+zip")
                        {
                            using (var bReader = new BinaryReader(upload.InputStream))
                            {
                                publication.Content = bReader.ReadBytes(upload.ContentLength);
                                var binaryData = publication.Content;
                                var memoStr = new MemoryStream(binaryData);
                                memoStr.Position = 0;
                                var epbk = EpubReader.ReadBook(memoStr);
                                var covImg = epbk.CoverImage;
                                if (covImg != null)
                                {
                                    using (var anotherMemStr = new MemoryStream())
                                    using (var coverImageStream = new MemoryStream(covImg))
                                    {
                                        var coverImage = Image.FromStream(coverImageStream);
                                        // Magic Numbers here (533 x 800) I here by deem this aspect ratio aesteticaly appealing :)
                                        Image resizedThumnail = ResizeImage(coverImage, 533, 800);
                                        resizedThumnail.Save(anotherMemStr, ImageFormat.Png);
                                        anotherMemStr.Position = 0;
                                        publication.Thumbnail = anotherMemStr.ToArray();
                                    }
                                }
                                //TODO if covImg is null, give thumbnail a boring stock pic from server

                                //borinImage= Url(../content/Images/LeatherCover.jpg);
                                if (covImg == null)
                                {
                                    using (var anotherMemStr = new MemoryStream())
                                    {
                                        string path = Server.MapPath("~/content/Images/LeatherCover.jpg");
                                        Image i = Image.FromFile(path);
                                        Image borinImage = ResizeImage(i, 533, 800);
                                        borinImage.Save(anotherMemStr, ImageFormat.Png);
                                        anotherMemStr.Position = 0;
                                        publication.Thumbnail = anotherMemStr.ToArray();
                                    }
                                }
                            }
                        }

                        if (upload.ContentType == @"application/pdf")
                        {
                            using (GhostscriptRasterizer rasterizer = new GhostscriptRasterizer())
                            using (BinaryReader br = new BinaryReader(upload.InputStream))
                            {
                                publication.Content = br.ReadBytes(upload.ContentLength);
                                byte[]binaryData=  publication.Content;
                                MemoryStream ms = new MemoryStream(binaryData);
                                ms.Position = 0;
                                rasterizer.Open(ms);
                                using (var mems = new MemoryStream())
                                {
                                    var img = rasterizer.GetPage(90, 90, 1);
                                    var anotherImg = ResizeImage(img, 533, 800);
                                    anotherImg.Save(mems, ImageFormat.Png);
                                    mems.Position = 0;
                                    publication.Thumbnail = mems.ToArray();
                                    mems.Close();
                                }
                                rasterizer.Close();


                            }
                        }
                        

                        var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
                        //manager.FindByNameAsync(User.Identity.Name)
                        
                        
                        if (magazine.Id == Guid.Empty)
                        {
                            var mag = new Magazine()
                            {
                                Publication = publication,
                                IssueNumber = magazine.IssueNumber,
                                Owner = currentUser

                                
                            };
                            mag.MagazineCategory = MagazineCategory.Entertainment;//Not mapped to DB but keeps from validation error
                           // currentUser.UserBooks = new List<Book> {bk};
                            _context.Magazines.Add(mag);
                            await _context.SaveChangesAsync();

                            return RedirectToAction("Publish");
                        }
                    }
               }
               
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError(" ", @"Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            
            return RedirectToAction("Publish");

        }
       [Authorize]
        public async Task<ActionResult> Edit(Guid id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            if (id == Guid.Empty)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Magazine magInDb = await _context.Magazines.FindAsync(id);
            ViewBag.IssueDate = magInDb.IssueNumber.ToShortDateString();
            if (magInDb == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole(RoleName.CanDoAnything))
            {
                return View("EditForm",magInDb);
            }
            if (magInDb.Owner.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View("EditForm",magInDb);
        }
       

        // POST: /Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit( Magazine magazine, HttpPostedFileBase upload)
        {
            var magInDb = await _context.Magazines.FindAsync(magazine.Id);
            
          
                try
                {
                    //ToDo: In the future use Automapper instead of this lengthy assignments 
                    magInDb.Publication.Description = magazine.Publication.Description;
                    magInDb.Publication.FileType = FileType.Magazine;
                    magInDb.Publication.Language = magazine.Publication.Language;
                    magInDb.Publication.Name = magazine.Publication.Name;
                    magInDb.Publication.Price = magazine.Publication.Price;
                    magInDb.Publication.Publisher = magazine.Publication.Publisher;
                    magInDb.Publication.Category = GetCategoryName(magazine.MagazineCategory) ?? magInDb.Publication.Category;
                    magInDb.Publication.Tags = magazine.Publication.Tags;
                    magInDb.IssueNumber = magazine.IssueNumber;
                    magInDb.MagazineCategory = MagazineCategory.Entertainment;// Not mapped to DB but keeps from db validation errors

                 
                    if (upload != null && upload.ContentLength > 0)
                    {
                       
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            magInDb.Publication.Content = reader.ReadBytes(upload.ContentLength);
                        }

                    }
                    
                        _context.Entry(magInDb).State = EntityState.Modified;

                        await _context.SaveChangesAsync();


                    return RedirectToAction("Index","Manage");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", @"Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            return View("EditForm", magazine);

        }
       [Authorize]
        public async Task<ActionResult> Details(string id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Magazine bookInDb =  await _context.Magazines.SingleOrDefaultAsync(m=>m.Id.ToString()==id);


            if (bookInDb == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
           
            ViewBag.format = bookInDb.Publication.MediaType == "application/epub+zip" ? "Epub" : "Pdf" ;
            ViewBag.length = ByteSize.FromBytes(bookInDb.Publication.Content.Length).ToString();

            return View("Details", bookInDb);
        }
       [Authorize]
        public async Task<ActionResult> Delete(Guid id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            if (id == Guid.Empty)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Magazine mag = await _context.Magazines.FindAsync(id);
            if (mag == null)
            {
                return HttpNotFound();
            }
            if (mag.Owner.Id != currentUser.Id && !User.IsInRole(RoleName.CanDoAnything))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            } 
            _context.Magazines.Remove(mag);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public ActionResult Publish()
        {
            Magazine mag = new Magazine();
            ModelState.SetModelValue("BookCategory",new ValueProviderResult(string.Empty,string.Empty,System.Globalization.CultureInfo.InvariantCulture));
            return PartialView("Publish",mag);
        }
        

        //[NonAction]
        [Authorize]
        public FileResult Buy(int?  id)
        {
            if (id != null)
            {
                try
                {
                    var currentUser = manager.FindById(User.Identity.GetUserId());

                    var purchasedBook = _context.UserMagazines
                        .Where(ub => ub.UserMagazineId == id && ub.ApplicationUser.Id == currentUser.Id && ub.Status =="PROCESSED")
                        .Select(ub=>ub.Magazine).Single();

                    if (purchasedBook == null)
                    {
                        return null;
                    }


                    var purchase = _context.UserMagazines.Single(ub => ub.UserMagazineId == id && 
                                                                       ub.ApplicationUser.Id == currentUser.Id 
                                                                       && ub.Status == "PROCESSED"
                                                                       && ub.Hits<4);
                    if (purchase == null)
                    {
                        return null;
                    }


                    if (purchase.Hits < 255)
                    {
                        purchase.Hits++;
                        _context.SaveChanges();
                    }
                    var bookInDb = purchasedBook;
                    if ((purchase.Hits<4))
                    {
                        //purchasedBook.
                        byte[] bookContents = bookInDb.Publication.Content;
                        var MIMEType = bookInDb.Publication.MediaType;
                        var extension = MIMEType == "application/epub+zip" ? "epub" : "pdf";


                        
                        
                        
                        return File(bookContents,MIMEType, bookInDb.Publication.Name+"."+extension);
                        

                    }
                }
                catch(RetryLimitExceededException /*dex*/)
                {
                    //Console.WriteLine(e);
                    ModelState.AddModelError("", @"Unable to process request.");
                    //throw;
                }

            }
            // ToDo Replace with error code not found
            return null;


        }
        public  ActionResult SearchByCategory(string searchString, int? page)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction("Index");
            }
//           
            
            var pageNumber = page ?? 1;
            //var books = from b in _context.Books select b;

            var books = _context.Magazines.Where(s => s.Publication.Category.Contains(searchString)).OrderBy(b=>b.Id);
            var onePageOfMagazines = books.ToPagedList(pageNumber, 10);
            ViewBag.onePageOfMagazines = onePageOfMagazines;

            return View("SearchPage");
        }
        [Authorize]
        public ActionResult MyMagazines()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var myBooks = _context.Magazines.Where(mb => mb.Owner.Id == currentUser.Id).ToList();

            return PartialView("Mymags_partial",myBooks);
            //return View("Mybooks",myBooks);

        }

        public ActionResult SearchByTags(string query, int?page)
        {
            var pageNumber = page ?? 1;
            var magazines = _context.Magazines.Where(m=>m.Publication.Tags.Contains(query)).ToList();
            var onePageOfMagazines = magazines.ToPagedList(pageNumber, 10);
            ViewBag.onePageOfMagazines = onePageOfMagazines;

            return View("SearchPage");
        }

        [Authorize]
        public async Task<ActionResult> ReplaceCover(Guid id, HttpPostedFileBase cover)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            if (id == Guid.Empty)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Magazine magInDb = await _context.Magazines.FindAsync(id);

            if (magInDb == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole(RoleName.CanDoAnything) || magInDb.Owner.Id == currentUser.Id)
            {
                if (cover != null && cover.ContentLength > 0)
                {

                    using (var anotherMemStr = new MemoryStream())
                    using (var reader = new System.IO.BinaryReader(cover.InputStream))
                    {
                        byte[] b  = reader.ReadBytes(cover.ContentLength);
                        using (var ms = new MemoryStream(b))
                        {
                            var image = Image.FromStream(ms);
                            var resized = ResizeImage(image, 533, 800);
                            resized.Save(anotherMemStr, ImageFormat.Png);
                            anotherMemStr.Position = 0;
                            magInDb.Publication.Thumbnail = anotherMemStr.ToArray();
                            
                        }
                        
                    }
                    _context.Entry(magInDb).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                

                return View("EditForm",magInDb);
            }
          
            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            
        }
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// Shamelessly lifted from StackOverflow
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        [NonAction]
        private string GetCategoryName(MagazineCategory value)
        {
            // Get the MemberInfo object for supplied enum value
            var memberInfo = value.GetType().GetMember(value.ToString());
            if (memberInfo.Length != 1)
                return null;

            // Get DisplayAttibute on the supplied enum value
            var displayAttribute = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
            if (displayAttribute == null || displayAttribute.Length != 1)
                return null;

            return displayAttribute[0].Name;
        }









    }

}