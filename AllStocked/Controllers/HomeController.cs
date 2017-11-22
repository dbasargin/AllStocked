using AllStocked.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AllStocked.Controllers
{
    public class HomeController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            string pageClass = filterContext.RouteData.Values["controller"].ToString()
                + "_" + filterContext.RouteData.Values["action"].ToString();
            ViewBag.PageClass = pageClass.ToLower();
        }
        /// <summary>
        /// Home page controller
        /// </summary>
        /// <returns>registration view</returns>
        public ActionResult Index()
        {
            //if current session is not null that means they are a member and need to send them to Products page
            if (Session["AccountID"] != null)
            {
                // redirect to index
                return RedirectToAction("Index", "Products");
            }
            //else send them to home page
            else
            {
                return View();

            }
        }

        [HttpPost]
        public ActionResult Index(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // if data valid add to database
                bool isSuccessful = DbHelper.RegisterMember(model);

                if (isSuccessful)
                {
                    Session["AccountID"] = DbHelper.GetAccountIdByEmail(model.Email);
                    Session["AccountType"] = 1;
                    return RedirectToAction("Index", "Home");
                }
                // todo return to home page
            }
            return View(model);
        }
        
    }
}