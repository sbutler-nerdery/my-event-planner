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
            PeopleInvited = new List<PersonViewModel>();
            WillBringTheseFoodItems = new List<FoodItemViewModel>();
            WillBringTheseGames = new List<GameViewModel>();
            AllEventFoodItems = new List<FoodItemViewModel>();
            AllEventGames = new List<GameViewModel>();
            PeopleWhoAccepted = new List<PersonViewModel>();
            PeopleWhoDeclined = new List<PersonViewModel>();
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
            if (model.FoodItems != null) model.FoodItems.ForEach(x => WillBringTheseFoodItems.Add(new FoodItemViewModel(x)));
            if (model.Games != null) model.Games.ForEach(x => WillBringTheseGames.Add(new GameViewModel(x)));
            if (model.RegisteredInvites != null) model.RegisteredInvites.ForEach(x => PeopleInvited.Add(new PersonViewModel(x)));
            if (model.UnRegisteredInvites != null) model.UnRegisteredInvites.ForEach(x =>
                {
                    if (x.Email != null)
                    {
                        PeopleInvited.Add(new PersonViewModel{ FirstName = x.FirstName, LastName = x.LastName, Email = x.Email, IsRegistered = false});
                    }

                    //if (x.FacebookId != null)
                    //{
                    //    var value = string.Format("{0}|{1} {2}", x.FacebookId, x.FirstName, x.LastName);
                    //    PeopleInvited.Add(value);                        
                    //}
                });
            if (model.PeopleWhoAccepted != null) model.PeopleWhoAccepted.ForEach(x => PeopleWhoAccepted.Add(new PersonViewModel(x)));
            if (model.PeopleWhoDeclined != null) model.PeopleWhoDeclined.ForEach(x => PeopleWhoDeclined.Add(new PersonViewModel(x)));
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
        public List<FoodItemViewModel> MyFoodItems { get; set; }
        /// <summary>
        /// Get or set a list of all the games that are being provided by the host
        /// </summary>
        public List<GameViewModel> MyGames { get; set; }
        /// <summary>
        /// Get or set the list of people who are invited to the event
        /// </summary>
        [Display(Name = "People who are invited")]
        public List<PersonViewModel> PeopleInvited { get; set; }
        /// <summary>
        /// Get or set the list of people who have accepted event invitations
        /// </summary>
        [Display(Name = "People who have accepted")]
        public List<PersonViewModel> PeopleWhoAccepted { get; set; }
        /// <summary>
        /// Get or set the list of people who have declined event invitations
        /// </summary>
        [Display(Name = "People who have declined")]
        public List<PersonViewModel> PeopleWhoDeclined { get; set; }

        public List<string> TimeList { get; set; }
        /// <summary>
        /// Get or set the person template used to send invitations via email
        /// </summary>
        public PersonViewModel EmailInvite { get; set; }
        /// <summary>
        /// Get or set the an un registered guest to be updated
        /// </summary>
        public PersonViewModel UpdateGuest { get; set; }
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
            dataModel.UnRegisteredInvites = new List<PendingInvitation>();
            return dataModel;
        }

        #endregion
    }
}