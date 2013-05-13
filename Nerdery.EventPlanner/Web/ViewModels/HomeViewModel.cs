using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Data.Models;

namespace Web.ViewModels
{
    /// <summary>
    /// This view model is only used on the index view of the home controller
    /// </summary>
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            MyEvents = new List<EventListItem>();
            MyInvitations = new List<EventListItem>();
            AmAttending = new List<EventListItem>();
            HaveDeclined = new List<EventListItem>();
        }
        /// <summary>
        /// Build out the 
        /// </summary>
        /// <param name="person"></param>
        public HomeViewModel(Person person) : this()
        {
            person.MyEvents.ForEach(x => MyEvents.Add(new EventListItem(x)));
            //Note: events that have not been accepted or declined will have null HasAccepted and HasDeclined values
            person.MyInvitations.ForEach(x => MyInvitations.Add(new EventListItem(x)));
            person.AmAttending.ForEach(x =>
                {
                    var item = new EventListItem(x) {HasAccepted = true, HasDeclined = false};
                    AmAttending.Add(item);
                });
            person.HaveDeclined.ForEach(x =>
                {
                    var item = new EventListItem(x) {HasAccepted = false, HasDeclined = true};
                    HaveDeclined.Add(item);
                });
        }

        /// <summary>
        /// Get or set a list of the events that the user is hosting
        /// </summary>
        public List<EventListItem> MyEvents { get; set; } 
        /// <summary>
        /// Get or set the events that the user has been invited to
        /// </summary>
        public List<EventListItem> MyInvitations { get; set; }
        /// <summary>
        /// Get or set the events that the user has accepted invitations to
        /// </summary>
        public List<EventListItem> AmAttending { get; set; }
        /// <summary>
        /// Get or set the events that the user has declined invitations for
        /// </summary>
        public List<EventListItem> HaveDeclined { get; set; }
    }

    #region Helpers

    /// <summary>
    /// This is a helper class that is used exclusively by the HomeViewModel
    /// </summary>
    public class EventListItem
    {
        public EventListItem(Event theEvent)
        {
            EventId = theEvent.EventId;
            Title = theEvent.Title;
        }

        public int EventId { get; set; }
        public string Title { get; set; }
        public bool? HasAccepted { get; set; }
        public bool? HasDeclined { get; set; }
    }

    #endregion
}