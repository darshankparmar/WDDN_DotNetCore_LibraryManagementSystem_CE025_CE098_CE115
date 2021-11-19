using DatabaseLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private LibraryDbEntities db = new LibraryDbEntities();

        public ActionResult Login()
        {
            if (!(string.IsNullOrEmpty(Convert.ToString(Session["UserID"]))))
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult LoginUser(string username, string password)
        {
            if (!(string.IsNullOrEmpty(Convert.ToString(Session["UserID"]))))
            {
                return RedirectToAction("Index");
            }

            try
            { 
                if(username != null && password != null)
                {
                    var finduser = db.UserTables.Where(u => u.UserName == username && u.Password == password && u.IsActive == true).ToList();
                    if(finduser.Count() == 1)
                    {
                        Session["UserID"] = finduser[0].UserID;
                        Session["UserTypeID"] = finduser[0].UserTypeID;
                        Session["EmployeeID"] = finduser[0].EmployeeID;
                        Session["UserName"] = finduser[0].UserName;
                        Session["Password"] = finduser[0].Password;

                        //UserTypeID    UserTypeName
                        //  1           Admin
                        //  2           Operator
                        //  3           Teacher
                        //  4           Student 

                        string url = string.Empty;
                        if (finduser[0].UserTypeID == 2)
                        {
                            return RedirectToAction("Dashboard");
                        }
                        else if (finduser[0].UserTypeID == 3)
                        {
                            return RedirectToAction("Dashboard");
                        }
                        else if (finduser[0].UserTypeID == 4)
                        {
                            return RedirectToAction("Dashboard");
                        }
                        else if (finduser[0].UserTypeID == 1)
                        {
                            url = "Dashboard";
                        }
                        else
                        {
                            url = "Dashboard";
                        }

                        return RedirectToAction(url);
                    }
                    else
                    {
                        Session["UserID"] = string.Empty;
                        Session["UserTypeID"] = string.Empty;
                        Session["UserName"] = string.Empty;
                        Session["Password"] = string.Empty;
                        Session["EmployeeID"] = string.Empty;
                        ViewBag.message = "Username and Password is incorrect!";
                    }
                }
                else
                {
                    Session["UserID"] = string.Empty;
                    Session["UserTypeID"] = string.Empty;
                    Session["UserName"] = string.Empty;
                    Session["Password"] = string.Empty;
                    Session["EmployeeID"] = string.Empty;
                    ViewBag.message = "Some Unexpected issue is occure please try again!";
                }
            }
            catch(Exception ex)
            {
                Session["UserID"] = string.Empty;
                Session["UserTypeID"] = string.Empty;
                Session["UserName"] = string.Empty;
                Session["Password"] = string.Empty;
                Session["EmployeeID"] = string.Empty;
                ViewBag.message = "Some Unexpected issue is occure please try again!";
            }
            return View("Login"); 
        }

        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        public ActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var issueBooks = db.IssueBookTables.Where(b => b.Status == true).ToList();
            ViewBag.IssueBooks = issueBooks.Count();

            var reserveBooks = db.IssueBookTables.Where(b => b.ReserveNoOfCopies == true).ToList();
            ViewBag.ReserveBooks = reserveBooks.Count();

            var returnPendingBooks = db.IssueBookTables.Where(b => b.Status == true || b.ReserveNoOfCopies == true).ToList();
            ViewBag.ReturnPendingBooks = returnPendingBooks.Count();

            var returnbooks = db.BookReturnTables.ToList();
            ViewBag.Returnbooks = returnbooks.Count();

            int fine = 0;
            var fineList = db.BookFineTables.Where(f => f.ReceiveAmount > 0).ToList();
            foreach (var item in fineList)
            {
                fine += (int)item.ReceiveAmount;
            }
            ViewBag.Fine = fine;

            int usertypeid = 0;
            if (Session["UserTypeID"] != null)
            {
                usertypeid = Convert.ToInt32(Convert.ToString(Session["UserTypeID"]));
            }
            if (usertypeid == 3002)
            {
                return RedirectToAction("Index", "ReserveBook", null);
            }


            return View();
        }

        public ActionResult About()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int usertypeid = 0;
            if (Session["UserTypeID"] != null)
            {
                usertypeid = Convert.ToInt32(Convert.ToString(Session["UserTypeID"]));
            }
            if (usertypeid == 3002)
            {
                return RedirectToAction("Index", "ReserveBook", null);
            }

            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Logout()
        {
            Session["UserID"] = string.Empty;
            Session["UserTypeID"] = string.Empty;
            Session["UserName"] = string.Empty;
            Session["Password"] = string.Empty;
            Session["EmployeeID"] = string.Empty;

            return RedirectToAction("Login");
        }

    }
}