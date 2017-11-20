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
    // This Metadata class allows me to add Data Annotations to my Project, While using a Ef Database First Design
    public class ProductMetadata
    {
        public int ProductID { get; set; }
        public int AccountID { get; set; }
        public Nullable<int> CategoryID { get; set; }

        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        
        //[Required]
        [Display(Name = "Par")]
        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "Par must be a whole number")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Cannot be below 0")]
        [GenericCompare(CompareToPropertyName = "Demand",
        OperatorName = GenericCompareOperator.GreaterThan,
            ErrorMessage="Par needs to be greater than demand")]
        public int Par { get; set; }

        [Display(Name = "Demand")]
        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "Demand must be a whole number")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Cannot be below 0")]
        public int Demand { get; set; }

        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "Supply must be a whole number")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Cannot be below 0")]
        public int Supply { get; set; }

        [StringLength(250, ErrorMessage = "Description is above Max Length")]
        public string Description { get; set; }

        public virtual Account Account { get; set; }
        public virtual Category Category { get; set; }
    }

    //Enum needed for custom Data Anotations
    public enum GenericCompareOperator
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    //This Class Adds Custom Data Annotations: GreaterThan, GreaterThanOrEqual, LessThan, and  LessThanOrEqual 
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
            if(value == null)
            {
                //return null;
                return new ValidationResult(base.ErrorMessage);

            }
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
            //string errorMessage = "Par needs than demand";//
            string errorMessage = this.FormatErrorMessage(metadata.DisplayName);
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