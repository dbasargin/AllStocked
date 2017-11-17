using AllStocked.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AllStocked.Controllers
{
    public class SettingsController : Controller
    {
        /// <summary>
        /// gets and modifies data for Settings View send updated model to View
        /// </summary>
        /// <returns></returns>
        public ActionResult Settings()
        {
            if (SessionHelper.IsMemberLoggedIn())
            {
                var model = new SettingsViewModel();
                var currentSessionId = SessionHelper.getAccountIdFromSession();
                
                using (var db = new AllStockedDBEntities())
                {
                    Account accountInSession = db.Accounts.Where(a => a.AccountID == currentSessionId).Single();
                    var listSecondaryAccountAccess = db.SecondaryAccountAccesses.Where(a => a.OwnerAccountID == currentSessionId).ToList();

                    //modify account into editAccount for view model
                    model.Account = new EditAccountViewModel(accountInSession);

                    //modify list secondaryAccountAccess into its view model form
                    model.ListSecondaryAccounts = new List<SecondaryAccountAccessViewModel>();

                    foreach (SecondaryAccountAccess secondaryAccount in listSecondaryAccountAccess)
                    {
                        model.ListSecondaryAccounts.Add(new SecondaryAccountAccessViewModel(secondaryAccount));
                    }

                }
                return View(model);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        ////returns EditAccountViewModel which DisplaysaAccount information that a user can update
        //[HttpGet]
        //public ActionResult EditAccount(EditAccountViewModel account)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        return View(account);
        //    }

        //    return RedirectToAction("Settings", "Settings");
        //}

        //Updates Account information
        [HttpPost]
        public ActionResult EditAccount(EditAccountViewModel account)
        {
            using(var db = new AllStockedDBEntities())
            {
                var currentSessionId = SessionHelper.getAccountIdFromSession();
                Account accountInSession = db.Accounts.Where(a => a.AccountID == currentSessionId).Single();
                accountInSession.FirstName = account.FirstName;
                accountInSession.LastName = account.LastName;
                accountInSession.AccountEmail = account.AccountEmail;
                try
                {
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "Error Processing your request");
                    throw ex;
                }
            }

            return RedirectToAction("Settings", "Settings");
        }


        //navigates to SecondaryAccountsList Partial class which displays 
        // a list of an Accounts secondary accounts.
        [HttpGet]
        public ActionResult SecondaryAccountList(List<SecondaryAccountAccessViewModel> listSecondaryAccounts)
        {
            return View(listSecondaryAccounts);
        }

        [HttpGet]
        public ActionResult CreateSecondaryAccountAccess()
        {
            return View();
        }

        /// <summary>
        /// This controller adds a new SecondaryAccountAccess row to the database
        /// </summary>
        /// <param name="secondaryAccountAccess"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateSecondaryAccountAccess(SecondaryAccountAccess secondaryAccountAccess)
        {
            if(DbHelper.DoesAccountExist(secondaryAccountAccess.SecondaryAccountEmail))
            {
                ModelState.AddModelError("", "Invalid Email");
            }
            else if (ModelState.IsValid)
            {
                secondaryAccountAccess.OwnerAccountID = SessionHelper.getAccountIdFromSession();
                secondaryAccountAccess.AccessToken = DbHelper.RandomString();
                secondaryAccountAccess.OwnerEnabled = true;
                secondaryAccountAccess.SecondaryEnabled = false;
                secondaryAccountAccess.LastEdited = DateTime.Now;

                using (var db = new AllStockedDBEntities())
                {
                    try
                    {
                        db.SecondaryAccountAccesses.Add(secondaryAccountAccess);
                        db.SaveChanges();
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Error Processing your request");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Error Processing your request");
            }

            return RedirectToAction("Settings", "Settings");


        }
    }
}