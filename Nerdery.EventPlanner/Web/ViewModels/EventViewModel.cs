using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ViewModels
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        /// <summary>
        /// Get or set the person who is coordinating this event
        /// </summary>
        public PersonViewModel Coordinator { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual List<FoodItemViewModel> FoodItems { get; set; }
        public virtual List<GameViewModel> Games { get; set; }
        public virtual List<PersonViewModel> PeopleInvited { get; set; }
        public virtual List<PersonViewModel> PeopleWhoAccepted { get; set; }
        public virtual List<PersonViewModel> PeopleWhoDeclined { get; set; }
    }
}