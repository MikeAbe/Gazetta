using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Gazzetta.Models;
using Gazzetta.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Gazzetta.Controllers
{
    public class ProfilesController : Controller
    {
        // GET: Profiles
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> manager;

        public ProfilesController()
        {
            _context = new ApplicationDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
        }

        // GET: Profiles
        [Authorize]
        public ActionResult ListUser()
        {
            var user = _context.Users.Find(User.Identity.GetUserId());
            UserEdit uE = new UserEdit
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            return View("~/Views/Manage/Index.cshtml",uE);

        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public ActionResult EditUser(/*[Bind(Include = "Name,Email,PhoneNumber")]*/UserEdit model)
        {
            
            if (ModelState.IsValid)
            {
                var anotherUser = _context.Users.FirstOrDefault(u => u.PhoneNumber == model.PhoneNumber || u.Email == model.Email || u.UserName == model.Email);
                var user = _context.Users.Find(User.Identity.GetUserId());
                if (anotherUser!= null)
                {
                    if (user != anotherUser)
                    {
                        ModelState.AddModelError(String.Empty, @"Email and Phone Number must be unique, Please check these fields");
                        return View("~/Views/Manage/Index.cshtml",model);
                    }
                    
                }
                

                user.Name = model.Name;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                user.UserName = model.Email;
                _context.SaveChanges();
                return RedirectToAction("Index","Manage",model);
               // return View("~/Views/Manage/Index.cshtml");


            }
           
            return View("~/Views/Manage/Index.cshtml",model);

        }

        [Authorize(Roles = RoleName.CanDoAnything)]
        public ActionResult AllUsers()
        {
            return PartialView("AllUsers",manager.Users.ToList());
            //return manager.Users.ToList();
        }

        [Authorize(Roles = RoleName.CanDoAnything)]
        public ActionResult VerifyPublishers(string publisherId)
        {
            if (!string.IsNullOrEmpty(publisherId))
            {
                var user = _context.Users.Find(publisherId);
                user.IsVerrified = !user.IsVerrified;
                _context.SaveChanges();
                return RedirectToAction("Index", "Manage");
            }
            return RedirectToAction("Index", "Manage");


        }

        [Authorize(Roles = RoleName.CanDoAnything)]
        public ActionResult DeleteUser(string userId)
        {
           if(!string.IsNullOrEmpty(userId))
           {
               var user = manager.FindById(userId);
               if (user != null)
               {
                   var roleMan = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                   var adminRole = roleMan.FindByName(RoleName.CanDoAnything);
                   if (manager.IsInRole(userId, RoleName.CanDoAnything))
                   {
                       var roles = manager.GetRoles(userId);
                       foreach (var role in roles)
                       {
                           manager.RemoveFromRole(userId, role);
                       }
                   }
//                   if (Roles.IsUserInRole(user.UserName, RoleName.CanDoAnything))
//                   {
//                       var roles = manager.GetRoles(userId);
//                       foreach (var role in roles)
//                       {
//                           manager.RemoveFromRole(userId, role);
//                       }
//                   }
                   var books = _context.Books.Where(b => b.Owner.Id == userId);
                   var mags = _context.Magazines.Where(m => m.Owner.Id == userId);
                   var ub = _context.UserBooks.Where(usb => usb.AppliationUserId == userId);
                   var um = _context.UserMagazines.Where(usm => usm.ApplicationUserId == userId);
                   _context.UserMagazines.RemoveRange(um);
                   _context.UserBooks.RemoveRange(ub);
                   _context.Books.RemoveRange(books);
                   _context.Magazines.RemoveRange(mags);


                    
               }

               _context.Users.Remove(user);
               _context.SaveChanges();
           }

           return RedirectToAction("Index", "Manage");

        }

        [Authorize(Roles = RoleName.CanDoAnything)]
        public ActionResult UserPurchases(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                //userId = "19ed2eb0-65e5-4897-9813-4764991e4579";
                var books = _context.UserBooks
                    .Where(ub => ub.Status == "PROCESSED" && ub.AppliationUserId == userId)
                   // .Select(ub => ub.Book)
                    .ToList();
                var mags = _context.UserMagazines
                    .Where(um => um.Status == "PROCESSED" && um.ApplicationUserId == userId ).Include(m=>m.Magazine)
                   // .Select(um => um.Magazine  )
                    .ToList();

                var allPurch = new PurchViewModel
                {
                    UserMagazines = mags,
                    UserBooks = books
                };
                var u = manager.FindById(userId);
                ViewBag.user = u.Name;


                return View(allPurch);
            }
           
            return RedirectToAction("Index", "Manage");

        }

        [Authorize(Roles = RoleName.CanDoAnything)]
        public ActionResult UserPublications(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var books = _context.Books.Where(mb => mb.Owner.Id == userId).ToList();
                var mags = _context.Magazines.Where(mm => mm.Owner.Id == userId).ToList();

                var allPurch = new PurchasesViewModel
                {
                    Books = books,
                    Magazines = mags
                };
                var u = manager.FindById(userId);
                ViewBag.user = u.Name;

                return View(allPurch);

            }
            return RedirectToAction("Index", "Manage");

        }

        [Authorize(Roles = RoleName.CanDoAnything)]
        public ActionResult PayPublishers()
        {
            var users =_context.Users.Where(u => u.Payable > 0).ToList();
            return PartialView("PayPublishers", users);
        }















    }
}