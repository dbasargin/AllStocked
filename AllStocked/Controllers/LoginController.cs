using AllStocked.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AllStocked.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //db code
                Account ActiveAccount = DbHelper.getAccount(model.Email, model.Password);
                if (ActiveAccount == null)
                {
                    return View(model); // todo add custom error messsage. user credentials not found.
                }

                // create session
                Session["AccountID"] = ActiveAccount.AccountID;
                // redirect to index
                return RedirectToAction("Index", "Products"); // Need to change this to send user to products page!!!
            }
            //return model with error messages
            return View(model);
        }
    }
}
