using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Data.Models;

namespace Web.ViewModels
{
    public class EditEventViewModel : EventBaseViewModel
    {
        #region Constructors

        public EditEventViewModel()
        {
            PeopleInvited = new List<string>();
            WillBringTheseFoodItems = new List<string>();
            WillBringTheseGames = new List<string>();
            AllEventFoodItems = new List<FoodItemViewModel>();
            AllEventGames = new List<GameViewModel>();
            PeopleWhoAccepted = new List<int>();
            PeopleWhoDeclined = new List<int>();
        }

        public EditEventViewModel(Event model) :this()
        {
            EventId = model.EventId;
            Title = model.Title;
            Description = model.Description;
            Location = model.Location;
            StartDate = model.StartDate;
            StartTime = model.StartDate.ToString("h:mm tt");
            EndTime = model.EndDate.ToString("h:mm tt");
            if (model.FoodItems != null) model.FoodItems.ForEach(x => WillBringTheseFoodItems.Add(x.FoodItemId.ToString()));
            if (model.Games != null) model.Games.ForEach(x => WillBringTheseGames.Add(x.GameId.ToString()));
            if (model.RegisteredInvites != null) model.RegisteredInvites.ForEach(x => PeopleInvited.Add(x.PersonId.ToString()));
            if (model.NonRegisteredInvites != null) model.NonRegisteredInvites.ForEach(x =>
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
            if (model.PeopleWhoAccepted != null) model.PeopleWhoAccepted.ForEach(x => PeopleWhoAccepted.Add(x.PersonId));
            if (model.PeopleWhoDeclined != null) model.PeopleWhoDeclined.ForEach(x => PeopleWhoDeclined.Add(x.PersonId));
        }

        #endregion

        #region Properties
        [Required]
        [Display(Name = "Event location")]
        public string Location { get; set; }
        /// <summary>
        /// Get or set the person who is coordinating this event
        /// </summary>
        public PersonViewModel Coordinator { get; set; }
        /// <summary>
        /// Get or set a list of all the food items being provided by the host
        /// </summary>
        public MultiSelectList MyFoodItems { get; set; }
        /// <summary>
        /// Get or set a list of all the games that are being provided by the host
        /// </summary>
        public MultiSelectList MyGames { get; set; }
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
        /// <summary>
        /// Get or set the control id for the listbox that will be updated when adding food items
        /// </summary>
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
            dataModel.RegisteredInvites = new List<Person>();
            dataModel.FoodItems = new List<FoodItem>();
            dataModel.Games = new List<Game>();
            dataModel.RegisteredInvites = new List<Person>();
            dataModel.PeopleWhoAccepted = new List<Person>();
            dataModel.PeopleWhoDeclined = new List<Person>();
            dataModel.NonRegisteredInvites = new List<PendingInvitation>();
            return dataModel;
        }

        #endregion
    }
}