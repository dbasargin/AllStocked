//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AllStocked
{
    using System;
    using System.Collections.Generic;
    
    public partial class Account
    {
        public Account()
        {
            this.Categories = new HashSet<Category>();
            this.Products = new HashSet<Product>();
            this.SecondaryAccountAccesses = new HashSet<SecondaryAccountAccess>();
        }
    
        public int AccountID { get; set; }
        public string FirstName { get; set; }
        public string AccountEmail { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
        public string RecoveryKey { get; set; }
        public string Password { get; set; }
        public string Hash { get; set; }
        public string LastName { get; set; }
    
        public virtual AccountType AccountType { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<SecondaryAccountAccess> SecondaryAccountAccesses { get; set; }
    }
}
