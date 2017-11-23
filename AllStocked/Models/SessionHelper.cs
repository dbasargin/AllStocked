using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllStocked.Models
{
    public static class SessionHelper
    {
        public static bool IsMemberLoggedIn()
        {
            var currSession = HttpContext.Current.Session["CurrentSession"] as MySession;
            if (currSession == null || currSession.AccountID < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void CreateSession(int id, int accountType)
        {
            var mySession = new MySession();
            mySession.AccountID = id;
            mySession.AccountType = accountType;
            HttpContext.Current.Session["CurrentSession"] = mySession;
        }
        //Overload with 3 parameters
        public static void CreateSession(int id, int accountType, int ownerAccountID)
        {
            var mySession = new MySession();
            mySession.AccountID = id;
            mySession.AccountType = accountType;
            mySession.AccountOwnerID = ownerAccountID;
            HttpContext.Current.Session["CurrentSession"] = mySession;
        }

        //Turns the current Session, Account Type to type Secondary
        public static void SessionAccountTypeToSecondary()
        {
            var currSession = HttpContext.Current.Session["CurrentSession"] as MySession;
            currSession.AccountType = 2;
            HttpContext.Current.Session["CurrentSession"] = currSession;
        }

        //Abandons Sessions
        public static void AbandonSession()
        {
            HttpContext.Current.Session.Remove("CurrentSession");
        }

        public static int GetAccountIdFromSession() {

            var currSession = HttpContext.Current.Session["CurrentSession"] as MySession;

            return currSession.AccountID;
        }

        public static int GetAccountTypeFromSession()
        {

            var currSession = HttpContext.Current.Session["CurrentSession"] as MySession;

            return currSession.AccountType;
        }

        public static int GetAccountOwnerIDFromSession()
        {
            var currSession = HttpContext.Current.Session["CurrentSession"] as MySession;

            return currSession.AccountOwnerID;
        }
    }

    public class MySession
    {
        public int AccountID { get; set; }
        public int AccountType { get; set; }
        public int AccountOwnerID { get; set; }
    }
}