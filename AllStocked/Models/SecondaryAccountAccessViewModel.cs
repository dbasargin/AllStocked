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
        [Display(Name = "Secondary Account Email")]
        public string SecondaryAccountEmail { get; set; }
        public string Status { get; set; }
        [Display(Name = "Last Edited")]
        public Nullable<System.DateTime> LastEdited { get; set; }

        public SecondaryAccountAccessViewModel(SecondaryAccountAccess model)
        {
            SecondaryAccountEmail = model.SecondaryAccountEmail;
            LastEdited = model.LastEdited;

            if (model.OwnerEnabled == true && model.SecondaryEnabled == true)
            {
                Status = "Active";
            }
            else if (model.OwnerEnabled == true && model.SecondaryEnabled == false)
            {
                Status = "Pending";
            }
            else
            {
                Status = "Suspended";
            }
        }
    }
    
}