using AllStocked.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AllStocked.Controllers
{
    public class LoginController : Controller
    {
        private AllStockedDBEntities db = new AllStockedDBEntities();

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

        // To Do: refactor this method.. Too many call to the db.
        [HttpPost]
        public ActionResult LogIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //db code
                Account currAccount = DbHelper.GetAccountByLogin(model.Email, model.Password);
                if (currAccount == null)
                {
                    //user credentials not found:
                    ModelState.AddModelError("", "Invalid Username or Password");
                    return View();
                }

                int accountType = DbHelper.GetAccountTypeById(currAccount.AccountID);
                
                // create session
                SessionHelper.createSession(currAccount.AccountID, accountType);

                //update Account lastLogin property
                DbHelper.UpdateUsersLastLogin(currAccount);

                // redirect to index
                return RedirectToAction("Index", "Products"); // Need to change this to send user to products page!!!
            }
            //return model with error messages

            return View(model);
        }

        /// <summary>
        /// allows user to create a password recovery sent to them by email.
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SendRecoveryKey()
        {
            return View();
        }

        /// <summary>
        /// validates user AccountEmail submission, if email is valid, creates RecoveryKey, store in database and emails
        /// it to the user.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendRecoveryKey(string email)
        {
            email = email.Trim();
            var account = db.Accounts.Where(u => u.AccountEmail == email).Single();

            if (account == null)
            {
                TempData["Message"] = "User does not exist.";
                return View();
            }
            else
            {
                //generate password RecoveryKey token and update database
                string token = DbHelper.RandomString();
                account.RecoveryKey = token;
                db.SaveChanges();
                
                //send mail
                try
                {

                    DbHelper.EmailRecoveryKey(email, token);
                    TempData["Message"] = "Mail Sent.";
                }
                catch (Exception ex)
                {
                    //To Do: log error
                    TempData["Message"] = "Error occured while sending email.";
                }
            }

            //send to RecoverPassword View
            return RedirectToAction("RecoverPassword", "Login");
        }

        /// <summary>
        /// The view for this controller has a form, a user fills out to
        /// change their password. They need to be emailed their recoveryKey 
        /// in order to complete this form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RecoverPassword()
        {
            return View();
        }

        /// <summary>
        /// Updates users password if Recovery Token is valid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecoverPassword(PasswordRecoveryViewModel model)
        {
            var account = db.Accounts.Where(u => u.AccountEmail == model.AccountEmail.Trim()).Single();
            
            //
            if (account == null)
            {
                ModelState.AddModelError("", "Invalid Email");
            }
            else if (account.RecoveryKey == model.RecoveryKey)
            {
                //Generate new Salt
                string newSalt = Crypto.GenerateSalt();
                //HashPassword with salt
                string hashedPassword = Crypto.Hash(newSalt + model.Password, "SHA256");

                //change Account property values
                account.Password = hashedPassword;
                account.Hash = newSalt;
                account.RecoveryKey = null;

                try
                {
                    db.SaveChanges();
                    // TO DO: Send email verification password has been changed
                }
                catch(Exception ex)
                {
                    //TO DO: log error eventually
                    throw ex;
                }

                //TO DO: May send to a new page displaying password change successfull
                return RedirectToAction("Login", "Login");

            }
            else
            {
                ModelState.AddModelError("", "Error processing your request");
            }

            return View();
        }
    }
}
