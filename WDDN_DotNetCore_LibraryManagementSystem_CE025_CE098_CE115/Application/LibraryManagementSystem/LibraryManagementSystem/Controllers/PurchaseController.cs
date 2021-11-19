using DatabaseLayer;
using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class PurchaseController : Controller
    {
        private LibraryDbEntities db = new LibraryDbEntities();

        public ActionResult NewPurchase()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            double totalamount = 0;
            var temppur = db.PurTemDetailsTables.ToList();

            foreach (var item in temppur)
            {
                totalamount += (item.Qty * item.UnitPrice);
            }
            ViewBag.TotalAmount = totalamount;

            return View(temppur);
        }

        [HttpPost]
        public ActionResult AddItem(int BID, int Qty, float Price)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var find = db.PurTemDetailsTables.Where(i => i.BookID == BID).FirstOrDefault();
            if(find == null)
            {
                if(BID > 0 && Qty > 0 && Price > 0 )
                {
                    var newitem = new PurTemDetailsTable()
                    {
                        BookID =BID,
                        Qty = Qty,
                        UnitPrice = Price
                    };

                    db.PurTemDetailsTables.Add(newitem);
                    db.SaveChanges();
                    ViewBag.Message = "Book Add Sucessfully";
                }
            }
            else
            {
                ViewBag.Message = "Already Exist! Plz Check";
            }
            return RedirectToAction("NewPurchase");
        }

        public ActionResult DeleteConfirm(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var book = db.PurTemDetailsTables.Find(id);
            if (book != null)
            {
                db.Entry(book).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                ViewBag.Message = "Deleted Successfully";
                return RedirectToAction("NewPurchase");
            }

            ViewBag.Message = "Some Unexpected issue is occure, please contact to concern person!";

            return View("NewPurchase");
        }

        [HttpGet]
        public ActionResult GetBooks()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            List<BookMV> list = new List<BookMV>();
            var booklist = db.BookTables.ToList();
            foreach(var book in booklist)
            {
                list.Add(new BookMV { BookName = book.BookName, BookID = book.BookID });
            }

            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CancelPurchase()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var list = db.PurTemDetailsTables.ToList();
            bool cancelstatus = false;

            foreach (var item in list)
            {
                db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                int noofrecords = db.SaveChanges();
                if(cancelstatus == false)
                {
                    if(noofrecords > 0)
                    {
                        cancelstatus = true;
                    }
                }
            }
            if (cancelstatus == true)
            {
                ViewBag.Message = "Purchase is Canceled.";
                return RedirectToAction("NewPurchase");
            }

            ViewBag.Message = "Some Unexpected issue is occure, please contact to concern person!";
            return RedirectToAction("NewPurchase");
        }

        public ActionResult SelectSupplier()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var purchasedetails = db.PurTemDetailsTables.ToList();
            if (purchasedetails.Count == 0)
            {
                ViewBag.Message = "Purchase Cart Empty!";
                return RedirectToAction("NewPurchase");
            }

            var suppliers = db.SupplierTables.ToList();
            return View(suppliers);
        }

        [HttpPost]
        public ActionResult PurchaseConfirm(FormCollection collection)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            int supplierid = 0;
            string[] keys = collection.AllKeys;

            foreach(var name in keys)
            {
                if(name.Contains("name"))
                {
                    string idname = name;
                    string[] valueids = idname.Split(' ');
                    supplierid = Convert.ToInt32(valueids[1]);
                }
            }

            var purchasedetails = db.PurTemDetailsTables.ToList();
            double totalamount = 0;
            foreach (var item in purchasedetails)
            {
                totalamount += item.Qty * item.UnitPrice; 
            }

            if(totalamount == 0)
            {
                ViewBag.Message = "Purchase Cart Empty";
                return View("NewPurchase");
            }

            var purchaseheader = new PurchaseTable();
            purchaseheader.SupplierID = supplierid;
            purchaseheader.PurchaseDate = DateTime.Now;
            purchaseheader.PurchaseAmount = totalamount;
            purchaseheader.UserID = userid;
            db.PurchaseTables.Add(purchaseheader);
            db.SaveChanges();

            foreach (var item in purchasedetails)
            {
                var purdetails = new PurchaseDetailTable()
                {
                    BookID = item.BookID,
                    PurchaseID = purchaseheader.PurchaseID,
                    Qty = item.Qty,
                    UnitPrice = item.UnitPrice
                };
                db.PurchaseDetailTables.Add(purdetails);
                db.SaveChanges();

                var updatebookstock = db.BookTables.Find(item.BookID);
                updatebookstock.TotalCopies += item.Qty;
                /*updatebookstock.Price += item.UnitPrice;*/
                db.Entry(updatebookstock).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            db.PurTemDetailsTables.ToList().ForEach(x => {
                db.Entry(x).State = System.Data.Entity.EntityState.Deleted;
            });
            db.SaveChanges();

            ViewBag.Message = "Purchase Confirm Successfully!";

            return RedirectToAction("AllPurchase");
        }

        public ActionResult AllPurchase()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var list = db.PurchaseTables.ToList();
            
            return View(list);
        }

        public ActionResult PurchaseDetails(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var purchaseDetails = db.PurchaseDetailTables.Where(a => a.PurchaseID == id);
            if (purchaseDetails == null)
            {
                return HttpNotFound();
            }
            return View(purchaseDetails);
        }
    }
}
