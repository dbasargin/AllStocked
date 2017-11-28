using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AllStocked.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "First")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6)]
        [Required]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Your Passwords must match")]
        [Display(Name = "Confirm")]
        public string ConfirmPassword { get; set; }
        
    }
}