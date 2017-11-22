using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AllStocked.Models
{
    /// <summary>
    /// SecondaryAccountAccess View Model Contains data needed to display Accounts list of Secondary Accounts
    /// </summary>
    public class SecondaryAccountAccessViewModel
    {
        public int SecondaryAccountID { get; set; }

        [Display(Name = "Owner Email")]
        public string OwnerEmail { get; set; }

        [Display(Name = "Secondary Account Email")]
        public string SecondaryAccountEmail { get; set; }

        public string Status { get; set; }

        [Display(Name = "Last Edited")]
        public Nullable<System.DateTime> LastEdited { get; set; }

        //Turns model into View Model
        public SecondaryAccountAccessViewModel(SecondaryAccountAccess model)
        {
            SecondaryAccountID = model.SecondaryAccountID;
            SecondaryAccountEmail = model.SecondaryAccountEmail;
            OwnerEmail = model.OwnerEmail;
            LastEdited = model.LastEdited;
            Status = model.getStatus();
        }    

        /// <summary>
        /// On The SecondaryAccountAccessList Partial View there is a button that needs to be calculated.
        /// If Account is Active or Pending the button needs to say Disable else Activate.
        /// This method makes this easier 
        /// </summary>
        /// <returns></returns>
        public string GetChangeStatusButtonName()
        {
            if(Status == "Pending" || Status ==  "Active")
            {
                return "Disable";
            }
            else
            {
                return "Activate";
            }
        }
    }
    
}