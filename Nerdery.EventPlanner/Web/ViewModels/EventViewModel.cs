using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.ViewModels
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Event location")]
        public string Location { get; set; }
        /// <summary>
        /// Get or set the person who is coordinating this event
        /// </summary>
        [Required]
        public PersonViewModel Coordinator { get; set; }
        [Required]
        [Display(Name = "Date")]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "Start time")]
        public DateTime StartTime { get; set; }
        [Required]
        [Display(Name = "End time")]
        public DateTime EndTime { get; set; }
        public List<FoodItemViewModel> FoodItems { get; set; }
        public List<GameViewModel> Games { get; set; }
        public List<PersonViewModel> PeopleInvited { get; set; }
        public List<PersonViewModel> PeopleWhoAccepted { get; set; }
        public List<PersonViewModel> PeopleWhoDeclined { get; set; }
    }
}