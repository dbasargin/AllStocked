using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllStocked.Models
{
    //This model will help scaffold the settings View with two partial views Account and a list of secondary accounts
    public class SettingsViewModel
    {
        public EditAccountViewModel Account { get; set; }
        public List<SecondaryAccountAccessViewModel> ListSecondaryAccounts { get; set; }
    }
}