using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Data.Models;

namespace Web.ViewModels
{
    public class EventViewModel
    {
        #region Constructors

        public EventViewModel()
        {
            PeopleInvited = new List<string>();
            FoodItemsSelected = new List<int>();
            GamesSelected = new List<int>();
            PeopleWhoAccepted = new List<int>();
            PeopleWhoDeclined = new List<int>();
        }

        public EventViewModel(Event model) :this()
        {
            EventId = model.EventId;
            Title = model.Title;
            Description = model.Description;
            Location = model.Location;
            StartDate = model.StartDate;
            StartTime = model.StartDate.ToString("h:mm tt");
            EndTime = model.EndDate.ToString("h:mm tt");
            model.FoodItems.ForEach(x => FoodItemsSelected.Add(x.FoodItemId));
            model.Games.ForEach(x => GamesSelected.Add(x.GameId));
            model.PeopleInvited.ForEach(x => PeopleInvited.Add(x.PersonId.ToString()));
            model.PendingInvitations.ForEach(x =>
                {
                    if (x.Email != null)
                    {
                        var value = string.Format("{0}|{1}|{2}", x.Email, x.FirstName, x.LastName);
                        PeopleInvited.Add(value);
                    }

                    if (x.FacebookId != null)
                    {
                        var value = string.Format("{0}|{1} {2}", x.FacebookId, x.FirstName, x.LastName);
                        PeopleInvited.Add(value);                        
                    }
                });
            model.PeopleWhoAccepted.ForEach(x => PeopleWhoAccepted.Add(x.PersonId));
            model.PeopleWhoDeclined.ForEach(x => PeopleWhoDeclined.Add(x.PersonId));
        }

        #endregion

        #region Properties

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
        public PersonViewModel Coordinator { get; set; }
        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? StartDate { get; set; }
        [Required]
        [Display(Name = "Start time")]
        public string StartTime { get; set; }
        [Required]
        [Display(Name = "End time")]
        public string EndTime { get; set; }
        public MultiSelectList FoodItems { get; set; }
        public MultiSelectList Games { get; set; }
        public List<int> FoodItemsSelected { get; set; }
        public List<int> GamesSelected { get; set; }
        /// <summary>
        /// Get or set the complete list of people who could be invited to an event
        /// </summary>
        public MultiSelectList PeopleList { get; set; }
        [Display(Name = "People who are invited")]
        public List<string> PeopleInvited { get; set; }
        public List<int> PeopleWhoAccepted { get; set; }
        public List<int> PeopleWhoDeclined { get; set; }
        public List<string> TimeList { get; set; }
        /// <summary>
        /// Get or set the person template used to send invitations via email
        /// </summary>
        public PersonViewModel EmailInvite { get; set; }
        /// <summary>
        /// Get or set the list of Facebook friends for the current user
        /// </summary>
        public List<PersonViewModel> FacebookFriends { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Get a data model from this view model. IMPORTANT: data model date / time info not populated here!
        /// </summary>
        /// <returns></returns>
        public Event GetDataModel()
        {
            var dataModel = new Event();
            dataModel.EventId = EventId;
            dataModel.Title = Title;
            dataModel.Description = Description;
            dataModel.Location = Location;
            dataModel.PeopleInvited = new List<Person>();
            dataModel.FoodItems = new List<FoodItem>();
            dataModel.Games = new List<Game>();
            dataModel.PeopleInvited = new List<Person>();
            dataModel.PeopleWhoAccepted = new List<Person>();
            dataModel.PeopleWhoDeclined = new List<Person>();
            dataModel.PendingInvitations = new List<PendingInvitation>();
            return dataModel;
        }

        #endregion
    }
}