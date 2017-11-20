using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllStocked.Models
{
    public class EditAccountViewModel
    {
        public int AccountID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountEmail { get; set; }
        public AccountType AccountType { get; set; }

        //Parameterless constructor
        public EditAccountViewModel(){ } 

        //Constructor(turn account into EditAccountViewModel)
        public EditAccountViewModel(Account account)
        {
            this.AccountID = account.AccountID;
            this.FirstName = account.FirstName;
            this.LastName = account.LastName;
            this.AccountEmail = account.AccountEmail;
            this.AccountType = account.AccountType;
        }

    }
}