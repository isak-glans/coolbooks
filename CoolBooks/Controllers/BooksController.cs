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
    public class BooksController : Controller
    {
        private CoolBooksDataModel db = new CoolBooksDataModel();

        // GET: Books
        [AllowAnonymous]
        public ActionResult Index(string bookGenre, string titleString, string authorString, string isbnString)
        {
            var GenreList = new List<string>();

            var GenreQuery = from g in db.Genres
                            where g.IsDeleted == false
                            orderby g.Name
                            select g.Name;

            GenreList.AddRange(GenreQuery.Distinct());
            ViewBag.bookGenre = new SelectList(GenreList);

            ViewBag.MyId = User.Identity.GetUserId();


            var books = db.Books.Include(b => b.AspNetUsers).Include(b => b.Authors).Include(b => b.Genres).Where(b => !b.IsDeleted);

            if (!String.IsNullOrEmpty(titleString))
            {
                books = books.Where(s => s.Title.Contains(titleString));
            }

            if (!string.IsNullOrEmpty(bookGenre))
            {
                books = books.Where(x => x.Genres.Name == bookGenre);
            }

            if (!String.IsNullOrEmpty(authorString))
            {
                books = books.Where(s => (s.Authors.FirstName + " " + s.Authors.LastName).Contains(authorString));
            }

            if (!String.IsNullOrEmpty(isbnString))
            {
                books = books.Where(s => s.ISBN.Equals(isbnString));
            }

            ViewBag.newReview = new Reviews();

            return View(books);
        }

        // GET: Books/Details/5
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public ActionResult Details(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var model = new DetailsBooksViewModel();
            model.Book = db.Books.Find(id);
            
            if (model.Book == null || model.Book.IsDeleted)
            {
                return HttpNotFound();
            }
            model.NewBookReivew = new Reviews();
            model.NewBookReivew.BookId = model.Book.Id;            

            model.UserReviews = db.Reviews.Where(r => r.IsDeleted == false && r.BookId == model.Book.Id).Include(r => r.AspNetUsers.Users).OrderBy(r => r.Created).ToList();
            //model.UserReviews = db.Reviews.Where(r => r.IsDeleted == false && r.BookId == model.Book.Id).Include(r => r.AspNetUsers.Users).OrderBy(r => r.Created).Skip((reviewPage - 1) * 10).Take(10).ToList();

            var pageNr = page ?? 1;

            model.CurrentReviewPage = pageNr;
            model.NrOfReviewPages = db.Reviews.Where(r => r.IsDeleted == false && r.BookId == model.Book.Id).Count();            
            model.NrOfReviewPages = (model.NrOfReviewPages -1)/ 10 + 1;

            model.UserIsAdmin   = User.IsInRole("Admin");
            model.UserId        = User.Identity.GetUserId();
            model.UserSignedIn = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            model.ReviewMenu = (PagedList<Reviews>)model.UserReviews.ToPagedList(pageNr, 10);            
            return View(model);
        }

        // GET: Books/Create        
        public ActionResult Create()
        {
            var model = new CreateBookViewModel();

            ViewBag.GenreId = new SelectList(db.Genres, "Id", "Name");
            return View(model);
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]        
        public ActionResult Create([Bind(Include = "AuthorFirstName,AuthorLastName,GenreId,Title,AlternativeTitle,Part,Description,ISBN,PublishDate,ImagePath")] CreateBookViewModel newBookData)
        {
            ViewBag.Error = "";
            if (ModelState.IsValid)
            {
                try
                {
                    storeNewBook(newBookData);
                    return RedirectToAction("Index");
                } catch (Exception e)
                { 
                    ViewBag.Error = "Bokens ISBN finns redan i databasen";
                }
            }
            
            ViewBag.GenreId = new SelectList(db.Genres, "Id", "Name", newBookData.GenreId);
            return View(newBookData);
        }

        // GET: Books/Edit/5  
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = db.Books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", books.UserId);
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "FirstName", books.AuthorId);
            ViewBag.GenreId = new SelectList(db.Genres, "Id", "Name", books.GenreId);
            return View(books);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,AuthorId,GenreId,Title,AlternativeTitle,Part,Description,ISBN,ImagePath")] Books books)
        {
            books.PublishDate = DateTime.Now;
            books.IsDeleted = false;

            ModelState.Remove("PublishDate");
            ModelState.Remove("IsDeleted");
            if (ModelState.IsValid)
            {
                db.Entry(books).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", books.UserId);
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "FirstName", books.AuthorId);
            ViewBag.GenreId = new SelectList(db.Genres, "Id", "Name", books.GenreId);
            return View(books);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = db.Books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return View(books);
        } 

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Books books = db.Books.Find(id);
            books.IsDeleted = true;
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

        private void storeNewBook(CreateBookViewModel newBookData)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                var apa = newBookData.AuthorFirstName;
                var author = db.Authors.Where(a => a.FirstName == newBookData.AuthorFirstName && a.LastName == newBookData.AuthorLastName).FirstOrDefault();
                var authorId = 0;
                try
                {
                    if (author == null)
                    {
                        var newAuthor = new Authors();
                        newAuthor.FirstName = newBookData.AuthorFirstName;
                        newAuthor.LastName = newBookData.AuthorLastName;
                        db.Authors.Add(newAuthor);
                        db.SaveChanges();

                        authorId = newAuthor.Id;
                    }
                    else
                    {
                        authorId = author.Id;
                    }

                    var strCurrentUserId = User.Identity.GetUserId();
                    Books book = mapCreateBooksViewModelToBook(newBookData, authorId, strCurrentUserId);
                    
                    db.Books.Add(book);
                    db.SaveChanges();
                    dbContextTransaction.Commit();

                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    throw;
                }
            }
        }

        private Books mapCreateBooksViewModelToBook(CreateBookViewModel model, int authorId, string userId)
        {
            var book = new Books();
            
            book.UserId = userId;
            book.AuthorId = authorId;
            book.Title = model.Title;
            book.Description = model.Description;
            book.Part = model.Part;
            book.ImagePath = model.ImagePath;
            book.ISBN = model.ISBN;
            book.PublishDate = model.PublishDate;
            book.GenreId = model.GenreId;
            book.AlternativeTitle = model.AlternativeTitle;
            book.AvgRating = 0;
            book.Created = DateTime.Now;
            book.IsDeleted = false;
            
            return book;
        }
    }
}
