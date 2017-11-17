using AllStocked.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AllStocked
{
    //This partial class allows me to add product Metadata to my Product model
    [MetadataType(typeof(ProductMetadata))]
    public partial class Product
    {
    }

    //Gives the ability to add functionality to SecondaryAccountAccess model 
    public partial class SecondaryAccountAccess
    {
        // Instructor accepts three parameters helpful in creating a secondaryAccount
        public SecondaryAccountAccess(int secondaryAccountID, int ownerID, string SecondEmail)
        {
            SecondaryAccountAccessID = secondaryAccountID;
            OwnerAccountID = ownerID;
            SecondaryAccountEmail = SecondEmail;
            AccessToken = DbHelper.RandomString();
            OwnerEnabled = true;
            SecondaryEnabled = false;
            LastEdited = DateTime.Now;
        }
    }

    public partial class SecondaryAccountAccess
    {
        public SecondaryAccountAccess(){ } // Parameterless constructor
    }

}