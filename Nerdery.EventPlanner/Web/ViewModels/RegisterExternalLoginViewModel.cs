﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.ViewModels
{
    /// <summary>
    /// This view model is used when registering a user via OAuth.
    /// </summary>
    public class RegisterExternalLoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Email address")]
        [EmailAddress]
        public string Email { get; set; }

        public string ExternalLoginData { get; set; }
        public int EventId { get; set; }
        public int PendingInvitationId { get; set; }
    }
}