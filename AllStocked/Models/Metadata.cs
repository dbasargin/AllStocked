using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using System.Resources;

namespace AllStocked
{
    public class ProductMetadata
    {
        public int ProductID { get; set; }
        public int AccountID { get; set; }
        public Nullable<int> CategoryID { get; set; }

        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        
        //[Required]
        [Display(Name = "Par")]
        //[GreaterThan("Demand", "Must be greater than demand")]
        [GenericCompare(CompareToPropertyName = "Demand",
        OperatorName = GenericCompareOperator.GreaterThan,
            ErrorMessage="Par needs to be greater than demand")]
        //[Range(0, Int32.MaxValue, ErrorMessage = "Cannot be below 0")]
        public int Par { get; set; }

        [Display(Name = "Demand")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Cannot be below 0")]
        public int Demand { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Cannot be below 0")]
        public int Supply { get; set; }

        [StringLength(250, ErrorMessage = "Description is above Max Length")]
        public string Description { get; set; }

        public virtual Account Account { get; set; }
        public virtual Category Category { get; set; }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GreaterThanAttribute : ValidationAttribute
    {
        string otherPropertyName;

        public GreaterThanAttribute(string otherPropertyName, string errorMessage)
            : base(errorMessage)
        {
            this.otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;
            try
            {
                // Using reflection we can get a reference to the other date property, in this example the project start date
                var otherPropertyInfo = validationContext.ObjectType.GetProperty(this.otherPropertyName);
                // Let's check that otherProperty is of type DateTime as we expect it to be
                if (otherPropertyInfo.PropertyType.Equals(new Int32().GetType()))
                {
                    int toValidate = (int)value;
                    int referenceProperty = (int)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
                    // if the end date is lower than the start date, than the validationResult will be set to false and return
                    // a properly formatted error message
                    if (toValidate.CompareTo(referenceProperty) < 1)
                    {
                        validationResult = new ValidationResult(ErrorMessageString);
                    }
                }
                else
                {
                    validationResult = new ValidationResult("An error occurred while validating the property. OtherProperty is not of type DateTime");
                }
            }
            catch (Exception ex)
            {
                // Do stuff, i.e. log the exception
                // Let it go through the upper levels, something bad happened
                throw ex;
            }

            return validationResult;
        }
    }

    public enum GenericCompareOperator
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    public sealed class GenericCompareAttribute : ValidationAttribute, IClientValidatable
    {
     
        private GenericCompareOperator operatorname = AllStocked.GenericCompareOperator.GreaterThanOrEqual;

        public string CompareToPropertyName { get; set; }
        public GenericCompareOperator OperatorName { get { return operatorname; } set { operatorname = value; } }

        public static object GenericCompareOperator { get; private set; }

        // public IComparable CompareDataType { get; set; }

        public GenericCompareAttribute() : base() { }
        //Override IsValid
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string operstring = (OperatorName == AllStocked.GenericCompareOperator.GreaterThan ?
            "greater than " : (OperatorName == AllStocked.GenericCompareOperator.GreaterThanOrEqual ?
            "greater than or equal to " :
            (OperatorName == AllStocked.GenericCompareOperator.LessThan ? "less than " :
            (OperatorName == AllStocked.GenericCompareOperator.LessThanOrEqual ? "less than or equal to " : ""))));
            var basePropertyInfo = validationContext.ObjectType.GetProperty(CompareToPropertyName);

            var valOther = (IComparable)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);

            var valThis = (IComparable)value;

            if ((operatorname == AllStocked.GenericCompareOperator.GreaterThan && valThis.CompareTo(valOther) <= 0) ||
                (operatorname == AllStocked.GenericCompareOperator.GreaterThanOrEqual && valThis.CompareTo(valOther) < 0) ||
                (operatorname == AllStocked.GenericCompareOperator.LessThan && valThis.CompareTo(valOther) >= 0) ||
                (operatorname == AllStocked.GenericCompareOperator.LessThanOrEqual && valThis.CompareTo(valOther) > 0))
                return new ValidationResult(base.ErrorMessage);
            return null;
        }
        #region IClientValidatable Members

        public IEnumerable<ModelClientValidationRule>
        GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string errorMessage = "Par needs than demand";//this.FormatErrorMessage(metadata.DisplayName);
            ModelClientValidationRule compareRule = new ModelClientValidationRule();
            compareRule.ErrorMessage = errorMessage;
            compareRule.ValidationType = "genericcompare";
            compareRule.ValidationParameters.Add("comparetopropertyname", CompareToPropertyName);
            compareRule.ValidationParameters.Add("operatorname", OperatorName.ToString());
            yield return compareRule;
        }

        #endregion
    }
    ////////////////////////
}