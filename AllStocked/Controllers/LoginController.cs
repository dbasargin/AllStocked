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
        AllStockedDBEntities db = new AllStockedDBEntities();

        public ActionResult LogOff()
        {
            SessionHelper.AbandonSession();
            return RedirectToAction("Index", "Home");
        }

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
                    //user credentials not found error view
                    return View("Error");
                }

                // create session
                Session["AccountID"] = ActiveAccount.AccountID;
                // redirect to index
                return RedirectToAction("Index", "Products"); // Need to change this to send user to products page!!!
            }
            //return model with error messages
            return View(model);
        }

        
        //[ValidateAntiForgeryToken]
        //public ActionResult RecoverPassword(string email)
        //{
        //    //check user existance
            

        //    var user = db.Accounts.Where(u => u.AccountEmail == email);

        //    if (user == null)
        //    {
        //        TempData["Message"] = "User Not exist.";
        //    }
        //    else
        //    {
        //        //generate password token
        //        //var token = WebSecurity.GeneratePasswordResetToken(UserName);
        //        //create url with above token
        //        //var resetLink = "<a href='" + Url.Action("ResetPassword", "Account", new { un = UserName, rt = token }, "http") + "'>Reset Password</a>";
        //        ////get user emailid
        //        //UsersContext db = new UsersContext();
        //        var emailid = email;
        //        //send mail
        //        string subject = "Password recovery";
        //        string body = "<b>password for account</b>" + email + ": " + user.; //edit it
        //        try
        //        {
        //            SendEMail(emailid, subject, body);
        //            TempData["Message"] = "Mail Sent.";
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["Message"] = "Error occured while sending email." + ex.Message;
        //        }
        //        //only for testing
        //        TempData["Message"] = resetLink;
        //    }

        //    return View();
        //}
    }
}
