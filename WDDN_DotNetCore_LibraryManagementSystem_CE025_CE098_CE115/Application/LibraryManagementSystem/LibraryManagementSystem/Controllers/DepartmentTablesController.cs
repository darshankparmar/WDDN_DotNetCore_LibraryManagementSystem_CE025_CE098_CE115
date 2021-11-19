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
    public class DepartmentTablesController : Controller
    {
        private LibraryDbEntities db = new LibraryDbEntities();

        // GET: DepartmentTables
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var departmentTables = db.DepartmentTables.Include(d => d.UserTable);
            return View(departmentTables.ToList());
        }

        // GET: DepartmentTables/Details/5
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
            DepartmentTable departmentTable = db.DepartmentTables.Find(id);
            if (departmentTable == null)
            {
                return HttpNotFound();
            }
            return View(departmentTable);
        }

        // GET: DepartmentTables/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        // POST: DepartmentTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DepartmentTable departmentTable)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            departmentTable.UserID = userid;

            if (ModelState.IsValid)
            {
                db.DepartmentTables.Add(departmentTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.UserTables, "UserID", "UserName", departmentTable.UserID);
            return View(departmentTable);
        }

        // GET: DepartmentTables/Edit/5
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
            DepartmentTable departmentTable = db.DepartmentTables.Find(id);
            if (departmentTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.UserTables, "UserID", "UserName", departmentTable.UserID);
            return View(departmentTable);
        }

        // POST: DepartmentTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DepartmentTable departmentTable)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            departmentTable.UserID = userid;

            if (ModelState.IsValid)
            {
                db.Entry(departmentTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.UserTables, "UserID", "UserName", departmentTable.UserID);
            return View(departmentTable);
        }

        // GET: DepartmentTables/Delete/5
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
            DepartmentTable departmentTable = db.DepartmentTables.Find(id);
            if (departmentTable == null)
            {
                return HttpNotFound();
            }
            return View(departmentTable);
        }

        // POST: DepartmentTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            DepartmentTable departmentTable = db.DepartmentTables.Find(id);
            db.DepartmentTables.Remove(departmentTable);
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
    }
}
