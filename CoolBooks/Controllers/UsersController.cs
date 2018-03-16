using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CoolBooks.Models;
using Microsoft.AspNet.Identity;

namespace CoolBooks.Controllers
{
    public class UsersController : Controller
    {
        private CoolBooksDataModel db = new CoolBooksDataModel();

        // GET: Users
        //public ActionResult Index()
        //{
        //    var users = db.Users.Include(u => u.AspNetUsers);
        //    return View(users.ToList());
        //}

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users theUser = db.Users.Find(id);
            if (theUser == null)
            {
                return HttpNotFound();
            }
            UserDetailViewModel model = new UserDetailViewModel();

            model.Address = theUser.Address;
            model.Birthdate = theUser.Birthdate;
            model.City = theUser.City;
            model.Country = theUser.Country;
            model.Created = theUser.Created;
            model.DisplayName = theUser.DisplayName;
            model.DisplayName = theUser.DisplayName;
            model.Email = theUser.Email;
            model.FirstName = theUser.FirstName;
            model.Gender = theUser.Gender;
            model.Info = theUser.Info;
            model.LastName = theUser.LastName;
            model.Phone = theUser.Phone;
            model.Picture = theUser.Picture;
            model.UserId = theUser.UserId;
            
            var strCurrentUserId = User.Identity.GetUserId();
            model.isMine = (strCurrentUserId != null & strCurrentUserId == theUser.UserId) ? true : false;

            ViewBag.Gender = model.Gender;

            return View(model);
        }

        // GET: Users/Create
        //public ActionResult Create()
        //{
        //    ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
        //    return View();
        //}

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "UserId,FirstName,LastName,Gender,Birthdate,Picture,Phone,Address,ZipCode,City,Country,Email,Info,DisplayName,Created,IsDeleted")] Users users)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Users.Add(users);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", users.UserId);
        //    return View(users);
        //}

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", users.UserId);
            ViewBag.GenderChoice = users.Gender;
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Userid,FirstName,LastName,Gender,Birthdate,Phone,Address,ZipCode,City,Country,Info,DisplayName")] Users user)
        {            
            ModelState.Remove("Email");
            if (ModelState.IsValid)
            {
                // Find the user
                Users theUser = db.Users.Find(user.UserId);

                theUser.FirstName   = user.FirstName;
                theUser.LastName    = user.LastName;
                theUser.Gender      = user.Gender;
                theUser.Birthdate   = user.Birthdate;
                theUser.Phone       = user.Phone;
                theUser.Address     = user.Address;
                theUser.ZipCode     = user.ZipCode;
                theUser.City        = user.City;
                theUser.Country     = user.Country;
                theUser.Info        = user.Info;
                theUser.DisplayName = user.DisplayName;                

                db.Entry(theUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details/"+ theUser.UserId);
                //return Redirect(Request.UrlReferrer.ToString()); // previous page
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", user.UserId);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Users users = db.Users.Find(id);
            users.IsDeleted = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult MyDisplayName ()
        {            
            ViewBag.displayName = "";
            ViewBag.userId = "";

            var myAspNetUserId = User.Identity.GetUserId();
            if (myAspNetUserId != null)
            {
                var displayName = db.Users.Find(myAspNetUserId).DisplayName;
                if (displayName != null)
                {
                    ViewBag.displayName = displayName;
                    ViewBag.userId = myAspNetUserId;
                }
            }            

            return PartialView("_displayName");
        }

    }
}
