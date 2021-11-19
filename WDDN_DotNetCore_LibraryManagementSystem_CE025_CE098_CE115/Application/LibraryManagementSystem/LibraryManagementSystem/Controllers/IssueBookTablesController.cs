using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DatabaseLayer;

namespace LibraryManagementSystem.Controllers
{
    public class IssueBookTablesController : Controller
    {
        private LibraryDbEntities db = new LibraryDbEntities();

        // GET: IssueBookTables
        public ActionResult IssueBooks() 
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var issueBookTables = db.IssueBookTables.Include(i => i.BookTable).Include(i => i.EmployeeTable).Include(i => i.UserTable).Where(b => b.Status == true && b.ReserveNoOfCopies == false);
            return View(issueBookTables.ToList());
        }

        public ActionResult ReserveBooks()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var issueBookTables = db.IssueBookTables.Include(i => i.BookTable).Include(i => i.EmployeeTable).Include(i => i.UserTable).Where(b => b.Status == false && b.ReserveNoOfCopies == true && b.ReturnDate > DateTime.Now);
            return View(issueBookTables.ToList());
        }

        public ActionResult ReturnPendingBooks()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            List<IssueBookTable> list = new List<IssueBookTable>();
            var issueBookTables = db.IssueBookTables.Where(b => b.Status == true || b.ReserveNoOfCopies == true).ToList();
            
            foreach(var item in issueBookTables)
            {
                var returndate = item.ReturnDate;
                int noofdays = (returndate - DateTime.Now).Days;
                if(noofdays <= 3)
                {
                    list.Add(new IssueBookTable 
                    {
                        BookID = item.BookID,
                        BookTable = item.BookTable,
                        Description = item.Description,
                        EmployeeID =item.EmployeeID,
                        EmployeeTable = item.EmployeeTable,
                        IssueBookID = item.IssueBookID,
                        IssueCopies = item.IssueCopies,
                        IssueDate = item.IssueDate,
                        ReserveNoOfCopies = item.ReserveNoOfCopies,
                        ReturnDate = item.ReturnDate,
                        Status = item.Status,
                        UserID = item.UserID,
                        UserTable = item.UserTable
                    });
                }
                
                
            }

            return View(list.ToList());
        }

        // GET: IssueBookTables/Details/5
        public ActionResult Details(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IssueBookTable issueBookTable = db.IssueBookTables.Find(id);
            if (issueBookTable == null)
            {
                return HttpNotFound();
            }
            return View(issueBookTable);
        }

        // GET: IssueBookTables/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.BookID = new SelectList(db.BookTables, "BookID", "BookTitle", "0");
            ViewBag.EmployeeID = new SelectList(db.EmployeeTables, "EmployeeID", "Email", "0");
            ViewBag.UserID = new SelectList(db.UserTables, "UserID", "UserName", "0");
            return View();
        }

        // POST: IssueBookTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IssueBookTable issueBookTable)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            issueBookTable.UserID = userid;

            if (ModelState.IsValid)
            {
                var find = db.IssueBookTables.Where(b => b.ReturnDate >= DateTime.Now && b.BookID == issueBookTable.BookID && (b.Status == true || b.ReserveNoOfCopies == true)).ToList();
                int issuecountbooks = 0;
                foreach(var item in find)
                {
                    issuecountbooks += item.IssueCopies;
                }

                var stockbooks = db.BookTables.Where(b => b.BookID == issueBookTable.BookID).FirstOrDefault();
                if((issuecountbooks == stockbooks.TotalCopies) || (issuecountbooks + issueBookTable.IssueCopies > stockbooks.TotalCopies))
                {
                    ViewBag.BookID = new SelectList(db.BookTables, "BookID", "BookTitle", issueBookTable.BookID);
                    ViewBag.EmployeeID = new SelectList(db.EmployeeTables, "EmployeeID", "Email", issueBookTable.EmployeeID);
                    ViewBag.Message = "Available Stock Only " + (stockbooks.TotalCopies - issuecountbooks);
                    return View("Create");
                }

                db.IssueBookTables.Add(issueBookTable);
                db.SaveChanges();
                ViewBag.Message = "Book Issue Successfully!";
                return RedirectToAction("IssueBooks");
            }

            ViewBag.BookID = new SelectList(db.BookTables, "BookID", "BookTitle", issueBookTable.BookID);
            ViewBag.EmployeeID = new SelectList(db.EmployeeTables, "EmployeeID", "FullName", issueBookTable.EmployeeID);
            ViewBag.UserID = new SelectList(db.UserTables, "UserID", "UserName", issueBookTable.UserID);
            return View(issueBookTable);
        }

        // GET: IssueBookTables/Edit/5
        public ActionResult Edit(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IssueBookTable issueBookTable = db.IssueBookTables.Find(id);
            if (issueBookTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookID = new SelectList(db.BookTables, "BookID", "BookTitle", issueBookTable.BookID);
            ViewBag.EmployeeID = new SelectList(db.EmployeeTables, "EmployeeID", "FullName", issueBookTable.EmployeeID);
            ViewBag.UserID = new SelectList(db.UserTables, "UserID", "UserName", issueBookTable.UserID);
            return View(issueBookTable);
        }

        // POST: IssueBookTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IssueBookTable issueBookTable)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            issueBookTable.UserID = userid;

            if (ModelState.IsValid)
            {
                db.Entry(issueBookTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookID = new SelectList(db.BookTables, "BookID", "BookTitle", issueBookTable.BookID);
            ViewBag.EmployeeID = new SelectList(db.EmployeeTables, "EmployeeID", "FullName", issueBookTable.EmployeeID);
            ViewBag.UserID = new SelectList(db.UserTables, "UserID", "UserName", issueBookTable.UserID);
            return View(issueBookTable);
        }

        // GET: IssueBookTables/Delete/5
        public ActionResult Delete(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IssueBookTable issueBookTable = db.IssueBookTables.Find(id);
            if (issueBookTable == null)
            {
                return HttpNotFound();
            }
            return View(issueBookTable);
        }

        // POST: IssueBookTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            IssueBookTable issueBookTable = db.IssueBookTables.Find(id);
            db.IssueBookTables.Remove(issueBookTable);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ApproveRequest(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var request = db.IssueBookTables.Find(id);
            request.ReserveNoOfCopies = false;
            request.Status = true;
            request.Description = "Approve";

            db.Entry(request).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ReserveBooks");
        }

        public ActionResult ReturnBook(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            var book = db.IssueBookTables.Find(id);
            int fine = 0;
            var returndate = book.ReturnDate;

            int noofdays = (DateTime.Now - returndate).Days;
            
            if(book.Status == true && book.ReserveNoOfCopies == false)
            {
                if(noofdays > 0)
                {
                    fine = 20 * noofdays * book.IssueCopies;
                }
                var returnbook = new BookReturnTable() {
                    BookID = book.BookID,
                    CurrentDate = DateTime.Now,
                    EmployeeID = book.EmployeeID,
                    IssueDate = book.IssueDate,
                    ReturnDate = book.ReturnDate,
                    UserID = userid
                };
                db.BookReturnTables.Add(returnbook);
                db.SaveChanges();
            }
            
            book.Status = false;
            book.ReserveNoOfCopies = false;
            db.Entry(book).State = EntityState.Modified;
            db.SaveChanges();

            if (fine > 0)
            {
                var addFine = new BookFineTable()
                {
                    BookID = book.BookID,
                    EmployeeID = book.EmployeeID,
                    FineAmount = fine,
                    FineDate = DateTime.Now,
                    NoOfDays = noofdays,
                    ReceiveAmount = 0,
                    UserID = userid
                };
                db.BookFineTables.Add(addFine);
                db.SaveChanges();
            }

            return RedirectToAction("IssueBooks");
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
