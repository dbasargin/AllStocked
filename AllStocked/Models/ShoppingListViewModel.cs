using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllStocked.Models
{
    public class ShoppingListViewModel
    {
        public int ProductID { get; set; }
        public int AccountID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string ProductName { get; set; }
        public int Par { get; set; }


        public int AmountToBuy { get; set; }

        public int Demand { get; set; }
        public int Supply { get; set; }
        public string Description { get; set; }
        public virtual Category Category { get; set; }
    }
}