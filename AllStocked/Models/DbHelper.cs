using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

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
        public static Account getAccount(string email, string passwordAttempt)
        {
            Account nullAccount = null;

            try
            {
                using (var db = new AllStockedDBEntities())
                {

                    Account Account = db.Accounts.Where(a => a.AccountEmail == email).Single();
                    
                    //account with previous to hashing
                    if (Account.Hash == null && passwordAttempt == Account.Password)
                    {
                        return Account;
                    }

                    else if ( Crypto.Hash(Account.Hash + passwordAttempt, "SHA256") == Account.Password)
                    { 
                        return Account;
                    }
                    else {
                        return nullAccount;
                    }
                }
            }
            catch
            {
                //throw new Exception("Issue Getting member from database");
                
                return nullAccount;
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
                using (var db = new AllStockedDBEntities())
                {
                    Account activeAccount = db.Accounts.Where(a => a.AccountEmail == email).Single();
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
            //Generate new Salt
            string newSalt = Crypto.GenerateSalt();
            //HashPassword with salt
            string hashedPassword = Crypto.Hash(newSalt + model.Password, "SHA256" );

            using (var db = new AllStockedDBEntities())
            {
                try
                {
                    var newAccount = new Account
                    {
                        AccountName = model.Name,
                        AccountEmail = model.Email,
                        Status = 1,                     //Initialize status to be active
                        LastLogin = DateTime.Now,
                        Type = 1,                       //Initialize Account type to be owner
                        Hash = newSalt,
                        Password = hashedPassword
                    };

                    db.Accounts.Add(newAccount);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        //this will update accounts lastlogin Property to When this method is called.
        public static void UpdateUsersLastLogin(Account account)
        {
            try
            {
                using (var db = new AllStockedDBEntities())
                {
                    account.LastLogin = DateTime.Now;
                    db.Accounts.Attach(account);
                    db.Entry(account).Property(x => x.LastLogin).IsModified = true;
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                throw ex;
                //to do : Log error: error type, date, message
            }

        }
    }
}