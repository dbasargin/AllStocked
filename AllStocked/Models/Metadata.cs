using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
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

        [GreaterThan("Demand", ErrorMessage = "Must be larger than Demand")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Cannot be below 0")]
        public int Par { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Cannot be below 0")]
        public int Demand { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Cannot be below 0")]
        public int Supply { get; set; }

        [StringLength(500, ErrorMessage = "Description is above Max Length")]
        public string Description { get; set; }

        public virtual Account Account { get; set; }
        public virtual Category Category { get; set; }
    }

    public class GreaterThanAttribute : ValidationAttribute
    {

        public GreaterThanAttribute(string otherProperty)
            : base("{0} must be greater than {1}")
        {
            OtherProperty = otherProperty;
        }

        public string OtherProperty { get; set; }

        public string FormatErrorMessage(string name, string otherName)
        {
            return string.Format(ErrorMessageString, name, otherName);
        }

        protected override ValidationResult
            IsValid(object firstValue, ValidationContext validationContext)
        {
            var firstComparable = firstValue as IComparable;
            var secondComparable = GetSecondComparable(validationContext);

            if (firstComparable != null && secondComparable != null)
            {
                if (firstComparable.CompareTo(secondComparable) < 1)
                {
                    object obj = validationContext.ObjectInstance;
                    var thing = obj.GetType().GetProperty(OtherProperty);
                    var displayName = (DisplayAttribute)Attribute.GetCustomAttribute(thing, typeof(DisplayAttribute));

                    return new ValidationResult(
                        FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        protected IComparable GetSecondComparable(
            ValidationContext validationContext)
        {
            var propertyInfo = validationContext
                                  .ObjectType
                                  .GetProperty(OtherProperty);
            if (propertyInfo != null)
            {
                var secondValue = propertyInfo.GetValue(
                    validationContext.ObjectInstance, null);
                return secondValue as IComparable;
            }
            return null;
        }
    }


}