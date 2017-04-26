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
    public class ProductsController : Controller
    {
        //get connection to db
        private AllStockedDBEntities db = new AllStockedDBEntities();

        public ActionResult UpdateSupply(int buy)
        {
            var getProd = db.Products.First(p => p.ProductID == buy);
            getProd.Supply = getProd.Par;
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("ShoppingList", "Products");
            }
            return RedirectToAction("ShoppingList", "Products");
        }

        //Show will accept search term from form and update product list
        [HttpPost]
        public ActionResult Show(string searchTerm)
        {
            searchTerm = searchTerm.ToString();
            int currentId = Convert.ToInt32(HttpContext.Session["AccountID"]);
            
            //find product names that match search term
            var products = db.Products.Where(p => p.AccountID == currentId && p.ProductName.Contains(searchTerm) ).OrderBy(p => p.Category.CategoryName).Include(p => p.Account).Include(p => p.Category);
            var productsList = products.ToList();
            
            // find and add product categories that match search term
            products = ( db.Products.Where(p => p.AccountID == currentId && p.Category.CategoryName.Contains(searchTerm)).Include(p => p.Account).Include(p => p.Category) );
            
            //if product doesnt already exist in list add to list
            foreach (var p in products)
            {
                bool doesNotExist = true;
                foreach (var i in productsList)
                {
                    if(p.ProductID == i.ProductID)
                    {
                        doesNotExist = false;
                    }
                }
                if (doesNotExist)
                {
                    productsList.Add(p);
                }
            }
            return View("Index" ,productsList);
        }

        // GET: Products that go on shopping list.. this is where supply is less or equal to demand.
        public ActionResult ShoppingList()
        {
            if (SessionHelper.IsMemberLoggedIn())
            {
                int currentId = Convert.ToInt32(HttpContext.Session["AccountID"]);
                //Get list of products that have a an account id that match current session.
                var products = db.Products.Where(p => p.AccountID == currentId && p.Supply <= p.Demand ).OrderBy(p => p.Category.CategoryName).Include(p => p.Account).Include(p => p.Category);

                List<ShoppingListViewModel> prodShoppingList = new List<ShoppingListViewModel>();
                foreach (var p in products)
                {
                    ShoppingListViewModel newListItem = new ShoppingListViewModel();
                    newListItem.AccountID = p.AccountID;
                    newListItem.Category = p.Category;
                    newListItem.ProductID = p.ProductID;
                    newListItem.ProductName = p.ProductName;
                    newListItem.Par = p.Par;
                    newListItem.Supply = p.Supply;
                    newListItem.Demand = p.Demand;
                    newListItem.Description = p.Description;
                    newListItem.AmountToBuy = p.Par - p.Supply;

                    prodShoppingList.Add(newListItem);

                }
                return View(prodShoppingList.ToList());
            }
            else
            {
                //if current session doesnt exist redirect user to home/register page.
                return RedirectToAction("Index", "Home");
            }
        }


        // GET: Products
        public ActionResult Index()
        {
            if (SessionHelper.IsMemberLoggedIn())
            {
                int currentId = Convert.ToInt32(HttpContext.Session["AccountID"]);
                //Get list of products that have a an account id that match current session.
                var products = db.Products.Where(p => p.AccountID == currentId).OrderBy(p => p.Category.CategoryName).Include(p => p.Account).Include(p => p.Category);
                return View(products.ToList());
            }
            else
            {
                //if current session doesnt exist redirect user to home/register page.
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (SessionHelper.IsMemberLoggedIn())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }
            else
            {
                //if current session doesnt exist redirect user to home/register page.
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            if (SessionHelper.IsMemberLoggedIn())
            {
                ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountName");
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
                return View();
            }
            else
            {
                //if current session doesnt exist redirect user to home/register page.
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,AccountID,CategoryID,ProductName,Par,Demand,Supply,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.AccountID = Convert.ToInt32(HttpContext.Session["AccountID"]);
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountName", product.AccountID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (SessionHelper.IsMemberLoggedIn())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountName", product.AccountID);
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                return View(product);
            }
            else
            {
                //if current session doesnt exist redirect user to home/register page.
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,AccountID,CategoryID,ProductName,Par,Demand,Supply,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.AccountID = Convert.ToInt32(HttpContext.Session["AccountID"]);
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountName", product.AccountID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (SessionHelper.IsMemberLoggedIn())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }
            else
            {
                //if current session doesnt exist redirect user to home/register page.
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
