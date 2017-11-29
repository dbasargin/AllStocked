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
        public static Account GetAccountByLogin(string email, string passwordAttempt)
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
                    else
                    {
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

        public static int GetAccountTypeById(int id)
        {
            
                using (var db = new AllStockedDBEntities())
                {

                    Account account = db.Accounts.FirstOrDefault(a => a.AccountID == id);

                    return account.AccountType.TypeID ;
                }
        }

        public static string GetEmailById(int id)
        {
            using (var db = new AllStockedDBEntities())
            {
                Account account = db.Accounts.FirstOrDefault(a => a.AccountID == id);

                return account.AccountEmail;
            }
        }

        /// <summary>
        /// Checks database if the account exists by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool DoesAccountExist(string email)
        {
            bool exists = false;

            using (var db = new AllStockedDBEntities())
            {
                Account account = db.Accounts.FirstOrDefault(a => a.AccountEmail == email);
                if(account != null)
                {
                    exists = true;
                }
            }

            return exists;
        }

        /// <summary>
        /// This retrieves the entire account by email later change to just account ID
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static int GetAccountIdByEmail(string email)
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
                throw new Exception("Could not get AccountID with an AccountEmail of: " + email);
                
            }
        }

        //return the first name and last name of account by given AccountID
        public static string GetAccountFullNameById(int id)
        {
            try
            {
                using (var db = new AllStockedDBEntities())
                {
                    Account account = db.Accounts.Where(a => a.AccountID == id).Single();
                    return account.FirstName + " " + account.LastName;
                }
            }
            catch
            {
                throw new Exception("Could not get AccountEmail with an AccountID of: " + id );

            }
        }

        /// <summary>
        /// Checks SecondaryAccountAccess table NOT Account.Type property
        /// important to check as to not make account secondary to multiple accounts
        /// </summary>
        /// <param name="secondaryEmail"></param>
        /// <returns></returns>
        public static bool IsAccountAlreadySecondary(string secondaryEmail)
        {
            try
            {
                using (var db = new AllStockedDBEntities())
                {
                    SecondaryAccountAccess secondaryAccount = db.SecondaryAccountAccesses.Where(a => a.SecondaryAccountEmail == secondaryEmail).Single();
                    if(secondaryAccount == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static int GetAccountOwnerIdBySecondaryId(int secondaryId)
        {

            using (var db = new AllStockedDBEntities())
            {

                SecondaryAccountAccess accessAccount = db.SecondaryAccountAccesses.FirstOrDefault(a => a.SecondaryAccountID == secondaryId);

                return accessAccount.OwnerAccountID;
            }
        }

        /// <summary>
        /// This method checks changes the OwnerEnabled fields to the opposite
        /// of what they currently are. Activates or deactivates secondary account.
        /// This also updates the lastEdited property to current time.
        /// </summary>
        /// <param name="secondaryEmail"></param>
        /// <returns></returns>
        public static bool ModifySecondaryAccountStatus(string secondaryEmail)
        {
            try
            {
                using (var db = new AllStockedDBEntities())
                {
                    SecondaryAccountAccess secondaryAccountAccess = db.SecondaryAccountAccesses.Where(a => a.SecondaryAccountEmail == secondaryEmail).Single();
                    Account secondaryAccount = db.Accounts.Where(a => a.AccountID == secondaryAccountAccess.SecondaryAccountID).Single();

                    if (secondaryAccountAccess.OwnerEnabled == true)
                    {
                        secondaryAccountAccess.OwnerEnabled = false;
                        secondaryAccount.AccountType = db.AccountTypes.First(a => a.TypeID == 1);
                    }
                    else
                    {
                        secondaryAccountAccess.OwnerEnabled = true;
                        secondaryAccount.AccountType = db.AccountTypes.First(a => a.TypeID == 2);
                    }

                    secondaryAccountAccess.LastEdited = DateTime.Now;
                    db.SaveChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        //Deletes the Access of a Secondary Account
        public static bool DeleteSecondaryAccessAccount( string secondaryEmail)
        {
            try
            {
                using (var db = new AllStockedDBEntities())
                {
                    //Remove from database
                    var secondaryRecord = db.SecondaryAccountAccesses.Where(a => a.SecondaryAccountEmail == secondaryEmail).First();

                    //Update Secondary Account to owner Account 
                    var account = db.Accounts.Where(a => a.AccountEmail == secondaryEmail).Single();
                    account.Type = 1;

                    db.SecondaryAccountAccesses.Remove(secondaryRecord);
                    db.SaveChanges();
                    return true;

                }
            }
            catch
            {
                //TO Do: log error
                return false;
            }
        }

        /// <summary>
        /// Checks whether access Token provided by user is valid. If it is valid current accounts  
        /// type changes to secondary and the SecondaryAccessAccount records SecondaryAccountEnabled property
        /// is changed to true.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>true if Access Token matches database if not returns false</returns>
        public static bool ValidateSecondaryAccount(string accessToken)
        {
            int currAccountId = SessionHelper.GetAccountIdFromSession();

            try
            {
                using (var db = new AllStockedDBEntities())
                {
                    
                    var account = db.Accounts.FirstOrDefault(a => a.AccountID == currAccountId);

                    var secondaryAccountAccessRecord = db.SecondaryAccountAccesses.FirstOrDefault(a => a.SecondaryAccountEmail == account.AccountEmail);

                    if (secondaryAccountAccessRecord.AccessToken == accessToken)
                    {
                        account.Type = 2;
                        secondaryAccountAccessRecord.SecondaryEnabled = true;
                        db.SaveChanges();

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
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
            string hashedPassword = Crypto.Hash(newSalt + model.Password.Trim(), "SHA256" );

            using (var db = new AllStockedDBEntities())
            {
                try
                {
                    var newAccount = new Account
                    {
                        FirstName = model.FirstName.Trim(),
                        LastName = model.LastName.Trim(),
                        AccountEmail = model.Email.Trim(),
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

        //To Do: Test email functionality
        /// <summary>
        /// Sends access key to join Owner Account via email
        /// This gives permissions to edit another persons inventory
        /// </summary>
        /// <param name="senderFullName"></param>
        /// <param name="secondaryEmail"></param>
        /// <param name="accessKey"></param>
        /// <returns></returns>
        public static bool EmailSecondaryAccessRequest(string senderFullName, string secondaryEmail, string accessKey)
        {
            //email message
            MailMessage mail = new MailMessage();
            mail.To.Add(secondaryEmail.Trim());
            mail.From = new MailAddress(Creds.EmailCreds(), "Message: Account Requests Action", System.Text.Encoding.UTF8);
            mail.Subject = "AllStocked Account Permission Request";
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = "Hello, You have been invited to become a secondary account on AllStocked <br>";
            mail.Body += "Please login and manage your accounts on your settings page.<br>";
            mail.Body += "Here is your Access Key: " + accessKey + " <br>";
            mail.Body += "For the Account Owner: " + senderFullName;

            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            //email Credentials for outlook
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(Creds.EmailCreds(), Creds.Psswrd());
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
            mail.From = new MailAddress(Creds.EmailCreds(), "Password Recovery", System.Text.Encoding.UTF8);
            mail.Subject = "AllStocked password recovery";
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = "Your Recovery Key is =  " + recoveryKey;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            //email Credential for outlook
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(Creds.EmailCreds(), Creds.Psswrd());
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