using DatabaseLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class ReturnBooksController : Controller
    {
        private LibraryDbEntities db = new LibraryDbEntities();

        // GET: ReturnBooks
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var returnbooks = db.BookReturnTables.ToList();

            return View(returnbooks);
        }
    }
}