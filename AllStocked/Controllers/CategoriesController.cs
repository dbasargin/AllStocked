using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AllStocked;
using AllStocked.Models;

namespace AllStocked.Controllers
{
    public class CategoriesController : Controller
    {
        private AllStockedDBEntities db = new AllStockedDBEntities();

        // GET: Categories
        public ActionResult Index()
        {
            if (SessionHelper.IsMemberLoggedIn())
            {
                int currentId = Convert.ToInt32(HttpContext.Session["AccountID"]);
                var categories = db.Categories.Where(p => p.AccountID == currentId).OrderBy(p=> p.CategoryName).Include(c => c.Account);
            return View(categories.ToList());
            }
            else
            {
                //if current session doesnt exist redirect user to home/register page.
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (SessionHelper.IsMemberLoggedIn())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                return View(category);
            }
            else
            {
                //if current session doesnt exist redirect user to home/register page.
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            if (SessionHelper.IsMemberLoggedIn())
            {
                ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountName");
                return View();
            }
            else
            {
                //if current session doesnt exist redirect user to home/register page.
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,AccountID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    category.AccountID = Convert.ToInt32(HttpContext.Session["AccountID"]);
                    db.Categories.Add(category);
                    db.SaveChanges();
                }
                catch{

                    return View("error");
                }
                return RedirectToAction("Index");
            }

            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountName", category.AccountID);
            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (SessionHelper.IsMemberLoggedIn())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountName", category.AccountID);
                return View(category);
            }
            else
            {
                //if current session doesnt exist redirect user to home/register page.
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,AccountID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                int currentAccount = Convert.ToInt32(HttpContext.Session["AccountID"]);
                using (db)
                {
                    //find category in database
                    Category editCategoryName = db.Categories.First(c => c.CategoryID == category.CategoryID);
                    //edit name with the parameter value
                    editCategoryName.CategoryName = category.CategoryName;
                    //and save
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountName", category.AccountID);
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (SessionHelper.IsMemberLoggedIn())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                return View(category);
            }
            else
            {
                //if current session doesnt exist redirect user to home/register page.
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);

            //First cahnge all product references To CategoryID to null
            using (db)
            {
                 db.Products.Where(p => p.CategoryID == id).ToList().ForEach(p => p.CategoryID = null);
                db.SaveChanges();
            
            //Then We should be able to remove category..
            db.Categories.Remove(category);
            db.SaveChanges();
            }

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
