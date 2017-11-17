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
            if (HttpContext.Current.Session["AccountID"] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void AbandonSession()
        {
            HttpContext.Current.Session.Remove("AccountID");
        }

        public static int getAccountIdFromSession(){

            return Convert.ToInt32(HttpContext.Current.Session["AccountID"]);
        }
    }
}