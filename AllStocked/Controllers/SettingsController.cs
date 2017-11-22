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
            //Error Handling for View 
            var errorMessage = TempData["ErrorMessage"] as string;
            if (errorMessage != null || errorMessage != "")
            {
                ModelState.AddModelError("", errorMessage);
            }

            if (SessionHelper.IsMemberLoggedIn())
            {
                var model = new SettingsViewModel();
                var currentSessionId = SessionHelper.getAccountIdFromSession();
                
                using (var db = new AllStockedDBEntities())
                {
                    Account accountInSession = db.Accounts.Where(a => a.AccountID == currentSessionId).Single();

                    var listSecondaryAccountAccess = db.SecondaryAccountAccesses.Where(a => a.OwnerAccountID == currentSessionId || a.SecondaryAccountEmail == accountInSession.AccountEmail).ToList();

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
            if (!SessionHelper.IsMemberLoggedIn())
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                using (var db = new AllStockedDBEntities())
                {
                    var currentSessionId = SessionHelper.getAccountIdFromSession();
                    Account accountInSession = db.Accounts.FirstOrDefault(a => a.AccountID == currentSessionId);
                    accountInSession.FirstName = account.FirstName;
                    accountInSession.LastName = account.LastName;
                    accountInSession.AccountEmail = account.AccountEmail;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        //To Do: Log Error (ex)
                        TempData["ErrorMessage"] = "Error Processing your request";
                    }
                }

                return RedirectToAction("Settings", "Settings");
            }

        }

        // navigates to SecondaryAccountsList Partial class which displays 
        // a list of an Accounts secondary accounts.
        [HttpGet]
        public ActionResult SecondaryAccountList(List<SecondaryAccountAccessViewModel> listSecondaryAccounts)
        {
            if (!SessionHelper.IsMemberLoggedIn())
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                return PartialView(listSecondaryAccounts);

            }
        }

        //Deletes the SecondaryAccountAccess record for the given email
        [HttpPost]
        public ActionResult DeleteSecondaryAccount(string email)
        {
            if (!SessionHelper.IsMemberLoggedIn())
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                bool accountDeleted = DbHelper.DeleteSecondaryAccessAccount(email);
                if (!accountDeleted)
                {
                    TempData["ErrorMessage"] = "Error deleting Secondary Access Account";
                }

                return RedirectToAction("Settings", "Settings");
            }
        }

        [HttpPost]
        public ActionResult ChangeSecondaryAccountStatus(string email)
        {
            bool isStatusChanged = DbHelper.ModifySecondaryAccountStatus(email);

            if (!isStatusChanged)
            {
                    TempData["ErrorMessage"] = "Error deleting Secondary Access Account";
            }

            return RedirectToAction("Settings", "Settings");
        }

        
        /// <summary>
        /// This Action method recieves an access token from the ui and if it matches
        /// the database, updates the current user accounts Type to secondary and give it permissions
        /// to edit an Owner Accounts inventory.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ValidateTokenSubmission(string accessToken)
        {

            int id = SessionHelper.getAccountIdFromSession();


            if (DbHelper.ValidateSecondaryAccount(accessToken))
            {
                
                SessionHelper.SessionAccountTypeToSecondary();
            }
            else
            {
                TempData["ErrorMessage"] = "Access Token Invalid";
            }

            return RedirectToAction("Settings", "Settings");
        }
        

        [HttpGet]
        public ActionResult CreateSecondaryAccountAccess()
        {
            return View();
        }

        //To do: Optimize this method: Queries db too many times.
        /// <summary>
        /// This controller adds a new SecondaryAccountAccess row to the database
        /// </summary>
        /// <param name="secondaryAccountAccess"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateSecondaryAccountAccess(SecondaryAccountAccess secondaryAccountAccess)
        {
            string errorMessage = "";
            //Check to see if the account exists
            if (!DbHelper.DoesAccountExist(secondaryAccountAccess.SecondaryAccountEmail))
            {
                errorMessage = "Invalid Email";
            }
            // make sure the account isnt already a secondary account
            else if (DbHelper.IsAccountAlreadySecondary(secondaryAccountAccess.SecondaryAccountEmail))
            {
                errorMessage = "This Account is already a secondary account.";
            }
            else
            {
                secondaryAccountAccess.OwnerAccountID = SessionHelper.getAccountIdFromSession();
                secondaryAccountAccess.OwnerEmail = DbHelper.GetEmailById(SessionHelper.getAccountIdFromSession());
                secondaryAccountAccess.SecondaryAccountID = DbHelper.GetAccountIdByEmail(secondaryAccountAccess.SecondaryAccountEmail);
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
                        //DbHelper.EmailSecondaryAccessRequest(senderName, DbHelper.EmailCreds(),  secondaryAccountAccess.AccessToken);
                    }
                    catch
                    {
                        errorMessage = "Error Processing your request";
                    }
                }
            }

            //Add Error Message
            if(!String.IsNullOrEmpty(errorMessage))
            {
                TempData["ErrorMessage"] = errorMessage;
            }

            return RedirectToAction("Settings", "Settings");
        }

    }
}