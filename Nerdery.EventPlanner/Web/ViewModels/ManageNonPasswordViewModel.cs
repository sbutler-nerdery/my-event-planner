using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.ViewModels
{
    /// <summary>
    /// This view model is used to store non-password user data
    /// </summary>
    public class ManageNonPasswordViewModel
    {
        public int UserId { get; set; }
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}