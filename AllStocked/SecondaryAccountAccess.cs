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
    
    public partial class SecondaryAccountAccess
    {
        public int SecondaryAccountAccessID { get; set; }
        public int OwnerAccountID { get; set; }
        public string SecondaryAccountEmail { get; set; }
        public string AccessToken { get; set; }
        public bool OwnerEnabled { get; set; }
        public bool SecondaryEnabled { get; set; }
        public Nullable<System.DateTime> LastEdited { get; set; }
    
        public virtual Account Account { get; set; }
    }
}
