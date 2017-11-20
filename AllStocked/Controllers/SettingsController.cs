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
        
        // navigates to SecondaryAccountsList Partial class which displays 
        // a list of an Accounts secondary accounts.
        [HttpGet]
        public ActionResult SecondaryAccountList(List<SecondaryAccountAccessViewModel> listSecondaryAccounts)
        {
            return View(listSecondaryAccounts);
        }

        //Deletes the SecondaryAccountAccess record for the given email
        [HttpPost]
        public ActionResult DeleteSecondaryAccount(string email)
        {

            bool accountDeleted = DbHelper.DeleteSecondaryAccessAccount(email);
            if (!accountDeleted)
            {
                ModelState.AddModelError("", "Error deleting Secondary Access Account");
            }

            return RedirectToAction("Settings", "Settings");
        }

        [HttpPost]
        public ActionResult ChangeSecondaryAccountStatus(string email)
        {
            bool isStatusChanged = DbHelper.ModifySecondaryAccountStatus(email);
            if (!isStatusChanged)
            {
                //log error AND
                ModelState.AddModelError("", "Error deleting Secondary Access Account");
            }

            return RedirectToAction("Settings", "Settings");
        }

        [HttpGet]
        public ActionResult CreateSecondaryAccountAccess()
        {
            return View();
        }

        /// <summary>
        /// This controller adds a new SecondaryAccountAccess row to the database
        /// To do: Optimize this method: Queries db many times.
        /// </summary>
        /// <param name="secondaryAccountAccess"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateSecondaryAccountAccess(SecondaryAccountAccess secondaryAccountAccess)
        {
            //Check to see if the account exists
            if(DbHelper.DoesAccountExist(secondaryAccountAccess.SecondaryAccountEmail))
            {
                ModelState.AddModelError("", "Invalid Email");
            }
            // make sure the account isnt already a secondary account
            else if (DbHelper.IsAccountAlreadySecondary(secondaryAccountAccess.SecondaryAccountEmail))
            {
                ModelState.AddModelError("", "This Account is already a secondary account.");
            }
            else
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

                        string senderName = DbHelper.GetAccountFullNameById(secondaryAccountAccess.OwnerAccountID);

                        // Sends email to requested account with the Access token to become secondary Account 
                        // TO Do: Production Send to secondaryAccountAccess.SecondaryAccountEmail:
                        // I am sending these emails to self for testing purposes. 
                        DbHelper.EmailSecondaryAccessRequest(senderName, DbHelper.EmailCreds(),  secondaryAccountAccess.AccessToken);
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Error Processing your request");
                    }
                }
            }

            return RedirectToAction("Settings", "Settings");


        }
    }
}