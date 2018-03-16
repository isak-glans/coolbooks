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
using PagedList;

namespace CoolBooks.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin")]
    public class ReviewsController : Controller
    {
        private CoolBooksDataModel db = new CoolBooksDataModel();

        // GET: Reviews
        public ActionResult Index(string bookTitle, string reviewer, int? page)
        {
            var reviews = db.Reviews.Include(r => r.AspNetUsers).Include(r => r.Books).Where(r => r.IsDeleted == false);

            if (!string.IsNullOrEmpty(bookTitle))
            {
                reviews = reviews.Where(r => r.Books.Title.Contains(bookTitle));
            }

            if (!string.IsNullOrEmpty(reviewer))
            {
                reviews = reviews.Where(r => r.AspNetUsers.Users.DisplayName.Equals(reviewer));
            }

            reviews = reviews.OrderBy(r => r.Id);

            var pageNumber = page ?? 1;
            return View(reviews.ToPagedList(pageNumber, 10));
        }

        // GET: Reviews/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reviews reviews = db.Reviews.Find(id);
            if (reviews == null || reviews.IsDeleted)
            {
                return HttpNotFound();
            }
            return View(reviews);
        }

        // GET: Reviews/Create
        [AllowAnonymous]
        public ActionResult Create(string bookId)
        {
            if (bookId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Books bookToReview = db.Books.Find(bookId);

            return View(bookToReview);
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Create([Bind(Include = "BookId,Title,Text,Rating")] Reviews reviews)
        {
            reviews.UserId = User.Identity.GetUserId();
            if (reviews.UserId == null) throw new Exception("User could not be found");
            reviews.Created = DateTime.Now;
            reviews.IsDeleted = false;

            ModelState.Remove("userid");
            ModelState.Remove("created");
            ModelState.Remove("isDeleted");
            if (ModelState.IsValid)
            {
                db.Reviews.Add(reviews);
                db.SaveChanges();

                if (reviews.Rating.HasValue)
                {
                    UpdateAvgRating(reviews);
                }
                
                return RedirectToAction("Details","Books", new { id = reviews.BookId } );                
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", reviews.UserId);
            //ViewBag.BookId = new SelectList(db.Books, "Id", "Title", reviews.Books.Title);
            return View(reviews);
        }

        // GET: Reviews/Edit/5
        [AllowAnonymous]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reviews reviews = db.Reviews.Find(id);
            if (reviews == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", reviews.UserId);
            //ViewBag.BookId = new SelectList(db.Books, "Id", "UserId", reviews.BookId);
            return View(reviews);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Edit([Bind(Include = "Id,BookId,UserId,Title,Text,Rating")] Reviews reviews)
        {
            ModelState.Remove("IsDeleted");
            ModelState.Remove("Created");
            if (ModelState.IsValid)
            {
                db.Entry(reviews).State = EntityState.Modified;
                db.SaveChanges();

                UpdateAvgRating(reviews);

                return RedirectToAction("Details", "Books", new { id = reviews.BookId });
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", reviews.UserId);
            ViewBag.BookId = new SelectList(db.Books, "Id", "UserId", reviews.BookId);
            return View(reviews);
        }

        // GET: Reviews/Delete/5
        [AllowAnonymous]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reviews reviews = db.Reviews.Find(id);
            if (reviews == null)
            {
                return HttpNotFound();
            }
            return View(reviews);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult DeleteConfirmed(int id)
        {
            ViewBag.Error = "";
            Reviews reviews = db.Reviews.Find(id);
            reviews.IsDeleted = true;
            try
            {
                db.SaveChanges();

                if (reviews.Rating.HasValue)
                {
                    UpdateAvgRating(reviews);
                }
                //return RedirectToAction("Index");
                return RedirectToAction("Details", "Books", new { id = reviews.BookId });
            }
            catch( Exception )
            {
                ViewBag.Error = "The entry is invalid data.";
                return View(reviews);
            }
        }

        public void UpdateAvgRating(Reviews reviews)
        {
            var myQuery = from r in db.Reviews
                          where (r.BookId == reviews.BookId && r.Rating.HasValue && !r.IsDeleted)
                          select (byte)r.Rating;

            int ratingCount = myQuery.Count();
            int sumRating = 0;

            foreach (var rating in myQuery)
                sumRating += rating;

            db.Books.Find(reviews.BookId).AvgRating = (double)sumRating / ratingCount;
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
