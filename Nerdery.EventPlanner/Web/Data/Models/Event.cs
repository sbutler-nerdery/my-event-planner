using System;
using System.Collections.Generic;

namespace Web.Data.Models
{
    public class Event
    {
        public int EventId { get; set; }
        /// <summary>
        /// Get or set a title for the event
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Get or set a short description of the event
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Get or set the event location
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Get or set the person who is coordinating this event
        /// </summary>
        public Person Coordinator { get; set; }
        /// <summary>
        /// Get or set the start date of the event
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Get or set the end date of the event
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Get or set the list of food items for the event
        /// </summary>
        public virtual List<FoodItem> FoodItems { get; set; }
        /// <summary>
        /// Get or set the list of games for the event
        /// </summary>
        public virtual List<Game> Games { get; set; }
        /// <summary>
        /// Get or set a list of people that have been invited to the event and are registered in the system
        /// </summary>
        public virtual List<Person> RegisteredInvites { get; set; }
        /// <summary>
        /// Get or set a list of people that have been invited to the event, but have not created user accounts yet.
        /// </summary>
        public virtual List<PendingInvitation> UnRegisteredInvites { get; set; }
        /// <summary>
        /// Get or set the list of people who have accepted invitations to the event
        /// </summary>
        public virtual List<Person> PeopleWhoAccepted { get; set; }
        /// <summary>
        /// Get or set the list of people who have declined invitations to the event
        /// </summary>
        public virtual List<Person> PeopleWhoDeclined { get; set; }
    }
}