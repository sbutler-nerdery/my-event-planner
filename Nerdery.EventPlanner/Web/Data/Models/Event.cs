using System;
using System.Collections.Generic;

namespace Web.Data.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        /// <summary>
        /// Get or set the person who is coordinating this event
        /// </summary>
        public Person Coordinator { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual List<FoodItem> FoodItems { get; set; }
        public virtual List<Game> Games { get; set; }
        public virtual List<Person> PeopleInvited { get; set; }
        public virtual List<Person> PeopleWhoAccepted { get; set; }
        public virtual List<Person> PeopleWhoDeclined { get; set; }
    }
}