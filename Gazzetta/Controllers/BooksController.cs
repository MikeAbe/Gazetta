/*
  A couple libraries you need to manipulate the uploaded book [to get the cover page Thumbnail, get the byte size...]:

    GhostScriptSharp A dotnet library that manipulate in-memory pdf obj (the book in this case)
    VersionOneEpub   A .Net library that manipulate in-memory epub obj (the book in this case)
    ByteSizeLib      A .Net library to calculate, convert between byte/megabyte...
    PagedList        A .Net library for easy pagination

 */




using System;
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
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ByteSizeLib;
using FluentValidation.Results;
using Gazzetta.FluentValidation;
using Gazzetta.Models;
using Gazzetta.ViewModels;
using Ghostscript.NET.Rasterizer;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using VersOne.Epub;
using PagedList;
using ValidationResult = FluentValidation.Results.ValidationResult;


namespace Gazzetta.Controllers
{
    public class BooksController : Controller
    {

        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> manager;

        public BooksController()
        {
            _context = new ApplicationDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));

        }
       
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();

        }
       
        
        // GET: UserBooks
        public ActionResult Index(int? page)
        {
//            if (User.IsInRole(RoleName.CanDoAnything))
//            {
//                return View("SuperView");
//            }
//            else
//            {
//                return View("normalView");
//            }
            var pageNumber = page ?? 1;
            var books = _context.Books.ToList();
            var onePageOfBooks = books.ToPagedList(pageNumber, 10);
            ViewBag.onePageOfBooks = onePageOfBooks;

            return View("SearchPage");
        }

        [Authorize(Roles = RoleName.CanDoAnything)]
        public ActionResult AllBooks()
        {
            var books = _context.Books.ToList();
            return PartialView("AllBooks_Partial",books);
            
        }
       

        public ActionResult GetThumbnail(int id)
        {
            var book = _context.Books.Find(id);
            if (book != null)
            {
                return File(book.Publication.Thumbnail, "png");
            }

            return HttpNotFound("oops, no cover found for this book");
        }
        [Authorize]
        public ActionResult PublishPartial()
        {
            //return View("Publish");
            return PartialView("publish_Partial");
        }
        [Authorize]
        public ActionResult Publish()
        {
            //return View("Publish");
            var model = new Book();
            ModelState.SetModelValue("BookCategory",new ValueProviderResult(string.Empty,string.Empty,System.Globalization.CultureInfo.InvariantCulture));
            return View("publish",model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //Asynchronous method
        public async Task<ActionResult> SaveBook(Book book, HttpPostedFileBase upload)
        {
            //Removed bc Id is not passed from the user form and results ModelState error if null
            ModelState.Remove("Id");
            //ModelState.Remove("")
            BookValidation bookValidation = new BookValidation();
            ValidationResult val = bookValidation.Validate(book);
            if (!val.IsValid)    //!ModelState.IsValid
            {
                foreach (ValidationFailure valFailure in val.Errors)
                {
                    ModelState.AddModelError(valFailure.PropertyName, valFailure.ErrorMessage);
                }

                return View("Publish", book);
            }


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
                    Category = GetCategoryName(book.BookCategory),
                    Tags = book.Publication.Tags,
                    MediaType = upload.ContentType
                };

                //use VersionOneEpub dll to extract epub thumbnail
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
                                Image resizedThumnail = ResizeImage(coverImage,533,800);
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
                                Image borinImage = ResizeImage(i,533,800);
                                borinImage.Save(anotherMemStr,ImageFormat.Png);
                                anotherMemStr.Position = 0;
                                publication.Thumbnail = anotherMemStr.ToArray();

                            }
                        }


                    }

                }

                //use  GhostScriptSharp dll to extract pdf thumbnail
                if (upload.ContentType == @"application/pdf")
                {
                    using (var rasterizer = new GhostscriptRasterizer())
                    using (var br = new BinaryReader(upload.InputStream))
                    {
                        publication.Content = br.ReadBytes(upload.ContentLength);
                        var binaryData = publication.Content;
                        var ms = new MemoryStream(binaryData);
                        ms.Position = 0;
                        rasterizer.Open(ms);
                        using (var mems = new MemoryStream())
                        {
                            // Magic Numbers here (90,90,1) 90 DPI for both width and Hight & Raterize the First page(1)
                            var img = rasterizer.GetPage(90, 90, 1);
                            // Magic Numbers here (533 x 800) I here by deem this aspect ratio aesteticaly appealing :)
                            var anotherImg = ResizeImage(img, 533, 800);
                            anotherImg.Save(mems, ImageFormat.Png);
                            mems.Position = 0;
                            publication.Thumbnail = mems.ToArray();
                            mems.Close(); //Redundant while using the 'using' key word but leaving it here for now
                        }

                        rasterizer.Close();
                    }

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
                    bk.BookCategory = BookCategory.Autobiography;// Not mapped to DB but keeps from validation errors
                
                    // currentUser.UserBooks = new List<Book> {bk};
                    
                    _context.Books.Add(bk);
                    //_context.UserBooks.Attach()
                    //bk.UserBooks.Add(currentUser);
                    
                    await _context.SaveChangesAsync();

                    return View("Publish");
                }


                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


                //unnecessary try catch...
//                catch (DbEntityValidationException dex)
//                {
//                    var error = dex.EntityValidationErrors.First().ValidationErrors.First();
//                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
//                    return View("PublishForm");
//                }
            }

                

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


        }

        public async Task<ActionResult> Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            //var user = _context.Users.Find(User.Identity.GetUserId());

           
            Book bookInDb = await _context.Books.FindAsync(id);


            if (bookInDb == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole(RoleName.CanDoAnything))
            {
                //ModelState.SetModelValue("BookCategory",new ValueProviderResult(string.Empty,string.Empty,System.Globalization.CultureInfo.InvariantCulture));

                return View("Edit",bookInDb);
            }
            if (bookInDb.Owner.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            ViewBag.Category = bookInDb.Publication.Category;
            
            return View("Edit",bookInDb);
        }

        // POST: /Book/Edit/#
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( Book book, HttpPostedFileBase upload)
        {
            var bookInDb = await _context.Books.FindAsync(book.Id);
            
          
                try
                {
                    //ToDo: In the future use Automapper instead of this lengthy assignments 
                    bookInDb.Author = book.Author;
                    bookInDb.Blurb = book.Blurb;
                    bookInDb.Publication.Description = book.Publication.Description;
                    bookInDb.Publication.FileType = FileType.Book;
                    bookInDb.Publication.Language = book.Publication.Language;
                    bookInDb.Publication.Name = book.Publication.Name;
                    bookInDb.Publication.Price = book.Publication.Price;
                    bookInDb.Publication.Publisher = book.Publication.Publisher;
                    bookInDb.Publication.Category =  GetCategoryName(book.BookCategory);
                    bookInDb.Publication.Tags = book.Publication.Tags;
                    bookInDb.BookCategory = BookCategory.Psychology;// Not mapped to DB but keeps from db validation errors

                 
                    if (upload != null && upload.ContentLength > 0)
                    {
                       
                        using (var reader = new BinaryReader(upload.InputStream))
                        {
                            bookInDb.Publication.Content = reader.ReadBytes(upload.ContentLength);
                        }

                    }
                    
                    
                    
                        _context.Entry(bookInDb).State = EntityState.Modified;
//                      
                       await _context.SaveChangesAsync();


                    return RedirectToAction("Index","Manage");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", @"Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            return View("Edit", book);

        }

        public async Task<ActionResult> Details(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book bookInDb = await _context.Books.FindAsync(id);

            if (bookInDb == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            ViewBag.format = bookInDb.Publication.MediaType == "application/epub+zip" ? "Epub" : "Pdf" ;
            ViewBag.length = ByteSize.FromBytes(bookInDb.Publication.Content.Length).ToString();

            return View("Details", bookInDb);
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
            var ub =  _context.UserBooks.Where(usb => usb.BookId == id );

            if (User.IsInRole(RoleName.CanDoAnything))
            {
                
                _context.UserBooks.RemoveRange(ub);
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            if (book.Owner.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            } 
            _context.UserBooks.RemoveRange(ub);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        
        public FileResult Buy(int? i)
        {
            if (i != null)
            {
                try
                {
                    var currentUser = manager.FindById(User.Identity.GetUserId());
                    var b = _context.UserBooks
                        .Where(ub => ub.UserBookId == i && ub.ApplicationUser.Id == currentUser.Id && ub.Status == "PROCESSED")
                        .Select(x => x.Book).Single();
                    if (b == null)
                    {
                        return null;
                    }
                    var purchase = _context.UserBooks.Single(ub =>ub.UserBookId == i && 
                                                                  ub.ApplicationUser.Id == currentUser.Id 
                                                                  && ub.Status == "PROCESSED"
                                                                  && ub.Hits<4);
                    if (purchase == null)
                    {
                        return null;
                    }

                    if (purchase.Hits < 5)
                    {
                        purchase.Hits++;
                        _context.SaveChanges();
                    }
                    var bookInDb = b;
                    if (purchase.Hits < 4)
                    {
                        byte[] bookContents = bookInDb.Publication.Content;
                        var MIMEType = bookInDb.Publication.MediaType;
                        var extension = MIMEType == "application/epub+zip" ? "epub" : "pdf";
                        return File(bookContents,MIMEType, bookInDb.Publication.Name+"."+extension);

                    }
                    
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", @"Unable to process request.");
                }
                
            }
            // ToDo Replace with error code not found
            return null;
            //return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            /*if (id != null)
            {
                try
                {
                    var currentUser = manager.FindById(User.Identity.GetUserId());

                    var purchasedBook = _context.UserBooks
                        .OrderByDescending(ub=>ub.PurchaseDate)
                        .Where(ub => ub.Book.Id == id && ub.ApplicationUser.Id == currentUser.Id && ub.Status == "PROCESSED")
                        .Select(ub=>ub.Book).FirstOrDefault();


                  
                    if (purchasedBook == null)
                    {
                        return null;
                    }
                    


                    var purchase = _context.UserBooks.OrderBy(ub=>ub.Hits).FirstOrDefault(ub =>ub.Book.Id == id && 
                                                                       ub.ApplicationUser.Id == currentUser.Id 
                                                                       && ub.Status == "PROCESSED"
                                                                       && ub.Hits<4);

                    
                    if (purchase != null && purchase.Hits < 5)
                    {
                        purchase.Hits++;
                        _context.SaveChanges();
                    }
                    
                    
                    var bookInDb = purchasedBook;
                    if (purchase != null && (bookInDb != null && purchase.Hits<4))
                    {
                        byte[] bookContents = bookInDb.Publication.Content;
                        var MIMEType = bookInDb.Publication.MediaType;
                        var extension = MIMEType == "application/epub+zip" ? "epub" : "pdf";
                        


                        
                        
                        
                        return File(bookContents,MIMEType, bookInDb.Publication.Name+"."+extension);
                        

                    }
                }
                catch(RetryLimitExceededException /*dex#1#)
                {
                    //Console.WriteLine(e);
                    ModelState.AddModelError("", @"Unable to process request.");
                    //throw;
                }

            }
            // ToDo Replace with error code not found
            return null;*/


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
        //[HttpGet]
        public  ActionResult SearchByCategory(string searchString, int?page)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction("Index");
            }
            var pageNumber = page ?? 1;
            //var books = from b in _context.Books select b;

            var books = _context.Books.Where(s => s.Publication.Category.Contains(searchString)).OrderBy(b=>b.Id);
            var onePageOfBooks = books.ToPagedList(pageNumber, 10);
            ViewBag.onePageOfBooks = onePageOfBooks;

            return View("SearchPage");
//           
           

        }

        public ActionResult SearchByTags(string query, int? page)
        {
            var pageNumber = page ?? 1;
            var books = _context.Books.Where(b=>b.Publication.Tags.Contains(query)).ToList();
            var onePageOfBooks = books.ToPagedList(pageNumber, 10);
            ViewBag.onePageOfBooks = onePageOfBooks;

            return View("SearchPage");

        }
        [AllowAnonymous]
        public ActionResult SearchBook(string query, int? page)
        {
            if (string.IsNullOrEmpty(query))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            
            var books = from b in _context.Books select b;
            var magazines = from m in _context.Magazines select m;

            if (! string.IsNullOrEmpty(query) )
            {
                
             
                var pageNumber = (page ?? 1);

                var bk = _context.Books.Where(b => b.Publication.Name.Contains(query)).OrderBy(b=>b.Id);
                var mg = _context.Magazines.Where(b => b.Publication.Name.Contains(query)).OrderBy(b=>b.Id);

//                IPagedList<SearchResultsViewModel> sR = null;
//                
                var searchResults = new SearchResultsViewModel
                {
                    Books = bk.ToPagedList(pageNumber,5),
                    Magazines = mg.ToPagedList(pageNumber,5)

                };
               
                //ViewBag.results = searchResults;
                ViewBag.query = query;

           
                return View(searchResults);

                //return View(searchResults);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }
        [Authorize]
        public ActionResult Purchases()
        {
            if (User.IsInRole(RoleName.CanDoAnything))
            {
                var books = _context.UserBooks
                    .Where(ub => ub.Status == "PROCESSED").Include(b=>b.Book).Include(u=>u.ApplicationUser)
                    //.Select(ub => ub.Book)
                    .ToList();
                var mags = _context.UserMagazines
                    .Where(um => um.Status == "PROCESSED" ).Include(m=>m.Magazine).Include(u=>u.ApplicationUser)
                    //.Select(um => um.Magazine )
                    .ToList();
                    
//                var allPurch = new PurchasesViewModel
//                {
//                    Books = books,
//                    Magazines = mags
//                };
                var allPurch = new PurchViewModel
                { 
                    UserBooks = books,
                    UserMagazines = mags

                };
                ViewBag.user = "All Users";

                return PartialView("Purchases",allPurch);

                //return PartialView("Purchases_Partial",allPurch);
            }

            var currentUser = manager.FindById(User.Identity.GetUserId());
            

                var myBooks = _context.UserBooks
                    .Where(ub => ub.AppliationUserId == currentUser.Id && ub.Status == "PROCESSED" /*&& ub.Hits<4*/)
                    .Include(b=>b.Book)
                    //.Select(ub => ub.Book)
                    .ToList();
                var myMags = _context.UserMagazines
                    .Where(um => um.ApplicationUserId == currentUser.Id && um.Status == "PROCESSED" /*&& um.Hits<4*/)
                    .Include(m=>m.Magazine)
                    //.Select(um => um.Magazine)
                    .ToList();
                    
//            var purch = new PurchasesViewModel
//            {
//                Books = myBooks,
//                Magazines = myMags
//            };
            var myPurch = new PurchViewModel
            { 
                UserBooks = myBooks,
                UserMagazines = myMags

            };
            ViewBag.user = currentUser.Name;
            

            return PartialView("Purchases",myPurch);
            //return View(myBooks);

        }

        public ActionResult MyBooks()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var myBooks = _context.Books.Where(mb => mb.Owner.Id == currentUser.Id).ToList();
            

            

//            var commentsOfMembers = context.MemberComments
//                .Where(mc => mc.Member.LastName == "Smith")
//                .Select(mc => mc.Comment)
//                .ToList();

            return PartialView("Mybooks_partial",myBooks);
            //return View("Mybooks",myBooks);

        }

        [Authorize]
        public async Task<ActionResult> ReplaceCover(int id, HttpPostedFileBase cover)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
            Book bookInDb = await _context.Books.FindAsync(id);

            if (bookInDb == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole(RoleName.CanDoAnything) || bookInDb.Owner.Id == currentUser.Id)
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
                            bookInDb.Publication.Thumbnail = anotherMemStr.ToArray();
                            
                        }
                        
                    }
                    _context.Entry(bookInDb).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index","Manage");

                }
                

                return View("Edit",bookInDb);
            }
          
            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            
        }
        private string GetCategoryName(BookCategory value)
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





















        /*        private Bitmap ResizeCover(Image image)
        {
            int width;
            int height;
            if (image.Width > 255 || image.Height > 255)
            {
                double zoom = Math.Max(image.Width / 255, image.Height / 255);
                width = (int)Math.Truncate(image.Width / zoom);
                height = (int)Math.Truncate(image.Height / zoom);
            }
            else
            {
                width = image.Width;
                height = image.Height;
            }
            Bitmap result = new Bitmap(width, height);
            Rectangle resultRectangle = new Rectangle(0, 0, width, height);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(image, resultRectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            }
            return result;
        }*/










    }
}