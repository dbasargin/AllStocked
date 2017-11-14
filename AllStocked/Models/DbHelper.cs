using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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

        //generates a random string of characters of length 8
        public static string RandomString()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }


        /// <summary>
        /// sends recoveryKey to userEmail
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="recoveryKey"></param>
        /// <returns></returns>
        public static bool EmailRecoveryKey(string userEmail, string recoveryKey)
        {
            //email message
            MailMessage mail = new MailMessage();
            mail.To.Add(userEmail.Trim());
            mail.From = new MailAddress("CompanyEmail@fake.com", "Password Recovery", System.Text.Encoding.UTF8);
            mail.Subject = "AllStocked password recovery";
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = "Your Recovery Key is =  " + recoveryKey;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            //email Credential for outlook
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("CompanyEmail@fake.com", "password");
            client.Port = 587;
            client.Host = "smtp-mail.outlook.com";
            client.EnableSsl = true;

            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                //TO DO: Some exception handling I viewed on the Stack, will look into it..
                Exception ex2 = ex;
                string errorMessage = string.Empty;
                while (ex2 != null)
                {
                    errorMessage += ex2.ToString();
                    ex2 = ex2.InnerException;
                }

                return false;
            }
            finally
            {
                mail.Dispose();
                client.Dispose();
            }   
        }


    }
}