using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllStocked.Models
{
    //Gets credentials needed for the applications
    public static class Creds
    {
        //To Do: Add this to .gitignore and test
        public static string EmailCreds()
        {
            return "CompanyEmail@fake.com";
        }

        public static string Psswrd()
        {
            return "password";
        }
    }
}