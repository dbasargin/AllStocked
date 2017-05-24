﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Foolproof;
using System.ComponentModel;

namespace AllStocked
{
    public class ProductMetadata
    {
        public int ProductID { get; set; }
        public int AccountID { get; set; }
        public Nullable<int> CategoryID { get; set; }

        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Cannot be below 0")]
        [GreaterThanOrEqualTo("Demand")]
        public int Par { get; set; }

        [LessThanOrEqualTo("Par")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Cannot be below 0")]
        public int Demand { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Cannot be below 0")]
        public int Supply { get; set; }

        [StringLength(500, ErrorMessage = "Description is above Max Length")]
        public string Description { get; set; }

        public virtual Account Account { get; set; }
        public virtual Category Category { get; set; }
    }

    
}