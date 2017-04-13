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
        public ActionResult Index()
        {
            return View();
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
                    Session["AccountID"] = DbHelper.getAccountIdByEmail(model.Email);
                    return RedirectToAction("Index", "Home");
                }
                // todo return to home page
            }
            return View(model);
        }

    }
}