using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace Web.ViewModels
{
    public class PersonViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePicUrl { get; set; }
        /// <summary>
        /// Get or set events that this user is coordinating
        /// </summary>
        public virtual List<EventViewModel> MyEvents { get; set; }
        /// <summary>
        /// Get or set events that this user is invited to
        /// </summary>
        public virtual List<EventViewModel> AmInvitedToThese { get; set; }
        /// <summary>
        /// Get or set events that this user is attending
        /// </summary>
        public virtual List<EventViewModel> AmAttending { get; set; }
        /// <summary>
        /// Get or set events that this user has declined
        /// </summary>
        public virtual List<EventViewModel> HaveDeclined { get; set; }
        public virtual List<FoodItemViewModel> MyFoodItems { get; set; }
        public virtual List<GameViewModel> MyGames { get; set; }
        public virtual List<PersonViewModel> MyFriends { get; set; }
        /// <summary>
        /// Get or set if the user will receive notifications via facebook. If false, send via email.
        /// </summary>
        public bool NotifyWithFacebook { get; set; }
    }
}
