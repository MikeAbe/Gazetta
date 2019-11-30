using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Gazzetta.Models;

namespace Gazzetta.Controllers
{
    public class PhoneValidatorController : Controller
    {
        private ApplicationDbContext _context;

        public PhoneValidatorController()
        {
            _context =  new ApplicationDbContext();
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult IsPhoneNumberUnique(string PhoneNumber)
        {
            return Json(! _context.Users.Any(u => u.PhoneNumber == PhoneNumber), JsonRequestBehavior.AllowGet);
           
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult IsEmailUnique(string Email)
        {
            return Json(! _context.Users.Any(u => u.Email == Email), JsonRequestBehavior.AllowGet);
           
        }

    }
}