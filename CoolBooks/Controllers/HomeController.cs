using CoolBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoolBooks.Controllers
{
    public class HomeController : Controller
    {
        private CoolBooksDataModel db = new CoolBooksDataModel();

        public ActionResult Index()
        {
            HomeBookViewModel model = new HomeBookViewModel();
            
            int numberOfBooks = db.Books.Where(b => !b.IsDeleted).Count();

            Random rnd = new Random();
            int rndBookNr = rnd.Next(1, numberOfBooks -1);

            model.RandomBook = db.Books.Where(b => b.IsDeleted == false).OrderBy(b => b.Created).Skip(rndBookNr).FirstOrDefault();            

            model.LatestBooks = db.Books.Where(b => b.IsDeleted == false).OrderBy(b => b.Created).Take(10).ToList();

            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}