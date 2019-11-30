using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Gazzetta.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Drawing.Imaging;
using Ghostscript.NET.Rasterizer;

namespace Gazzetta.Controllers
{
    public class CustomersController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> manager;

        public CustomersController()
        {
            _context = new ApplicationDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));

        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Customer
        public ActionResult Index()
        {
            var books = _context.Books.ToList();

            return View(books);
        }


        public ActionResult Publish()
        {
            return View("PublishForm");
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        //Asynchronous method
        public async Task<ActionResult> SaveBook(Book book, HttpPostedFileBase upload)
        {            
            try
            {
                ModelState.Remove("Id");
                //Save a new Book
               if (ModelState.IsValid )
               {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        var publication = new Publication
                        {
                            Description = book.Publication.Description,
                            FileType = FileType.Book,
                            Language = book.Publication.Language,
                            Name = book.Publication.Name,
                            Price = book.Publication.Price,
                            Publisher = book.Publication.Publisher,
                            Category = book.Publication.Category,
                        };
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
                                img.Save(mems, ImageFormat.Jpeg);
                                mems.Position = 0;
                                publication.Thumbnail = mems.ToArray();
                                mems.Close();
                            }
                            rasterizer.Close();


                        }

                        var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
                        //manager.FindByNameAsync(User.Identity.Name)
                        
                        if (book.Id == 0)
                        {
                            var bk = new Book
                            {
                                Publication = publication,
                                Author = book.Author,
                                Blurb = book.Blurb,
                                Owner = currentUser
                                
                            };

                            _context.Books.Add(bk);
                            await _context.SaveChangesAsync();

                            return RedirectToAction("Publish" ,"Customers");
                        }
                    }
               }
               
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError(" ", @"Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View("PublishForm");

        }

        public async Task<ActionResult> Edit(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book bookInDb = await _context.Books.FindAsync(id);

            if (bookInDb == null)
            {
                return HttpNotFound();
            }
            if (bookInDb.Owner.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View("EditForm",bookInDb);
        }

        // POST: /Book/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( Book book, HttpPostedFileBase upload)
        {
            var bookInDb = await _context.Books.FindAsync(book.Id);
            
          
                try
                {
                    //ToDo: In the future use Automapper instead of this lengthy assignment 
                    bookInDb.Author = book.Author;
                    bookInDb.Blurb = book.Blurb;
                    bookInDb.Publication.Description = book.Publication.Description;
                    bookInDb.Publication.FileType = FileType.Book;
                    bookInDb.Publication.Language = book.Publication.Language;
                    bookInDb.Publication.Name = book.Publication.Name;
                    bookInDb.Publication.Price = book.Publication.Price;
                    bookInDb.Publication.Publisher = book.Publication.Publisher;
                    bookInDb.Publication.Category = book.Publication.Category;
                 
                    if (upload != null && upload.ContentLength > 0)
                    {
                       
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            bookInDb.Publication.Content = reader.ReadBytes(upload.ContentLength);
                        }

                    }
                    
                    _context.Entry(bookInDb).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", @"Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            return View("EditForm", book);

        }
       
         

        public async Task<ActionResult> Delete(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            if (book.Owner.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            } 
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        
        public FileResult Buy(int? id)
        {
            if (id != null)
            {
                try
                {
                    var bookInDb = _context.Books.Single(x => x.Id == id);
                    if (bookInDb != null)
                    {
                        byte[] bookContents = bookInDb.Publication.Content;
                        return File(bookContents, /*System.Net.Mime.MediaTypeNames.Application.Octet*/"text/pdf",
                                    bookInDb.Publication.Name);
                    }
                }
                catch(RetryLimitExceededException /*dex*/)
                {
                    //Console.WriteLine(e);
                    ModelState.AddModelError("", @"Unable to process request.");
                    //throw;
                }

            }

            return null;


        }
        

    }
}