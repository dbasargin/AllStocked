using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllStocked.Models
{
    public static class DbHelper
    {
        /// <summary>
        /// Gets account if email and password match database.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Account getAccount(string email, string password)
        {
            try
            {
                using (var context = new AllStockedDBEntities())
                {
                    Account activeAccount = context.Accounts.Where(a => a.AccountEmail == email && a.AccountPassword == password).Single();
                    return activeAccount;
                }
            }
            catch
            {
                throw new Exception("Issue Getting member from database");
            }
        }
        /// <summary>
        /// This retrieves the entire account by email later change to just account ID
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static int getAccountIdByEmail(string email)
        {
            try
            {
                using (var context = new AllStockedDBEntities())
                {
                    Account activeAccount = context.Accounts.Where(a => a.AccountEmail == email).Single();
                    return activeAccount.AccountID;
                }
            }
            catch
            {
                throw new Exception("Issue Getting member from database");
            }
        }

        /// <summary>
        /// Register new member
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool RegisterMember(RegisterViewModel model)
        {
            using (var context = new AllStockedDBEntities())
            {
                try
                {
                    var newAccount = new Account
                    {
                        AccountName = model.Name,
                        AccountEmail = model.Email,
                        AccountPassword = model.Password
                    };

                    context.Accounts.Add(newAccount);
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}