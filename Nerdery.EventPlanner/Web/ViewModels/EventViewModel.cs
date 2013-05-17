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
            FoodItems = new List<FoodItemViewModel>();
            Games = new List<GameViewModel>();
            PeopleInvited = new List<int>();
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
            model.FoodItems.ForEach(x => FoodItems.Add(new FoodItemViewModel(x)));
            model.Games.ForEach(x => Games.Add(new GameViewModel(x)));
            model.PeopleInvited.ForEach(x => PeopleInvited.Add(x.PersonId));
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
        public List<FoodItemViewModel> FoodItems { get; set; }
        public List<GameViewModel> Games { get; set; }
        /// <summary>
        /// Get or set the complete list of people who could be invited to an event
        /// </summary>
        public MultiSelectList PeopleList { get; set; }
        [Display(Name = "People who are invited")]
        public List<int> PeopleInvited { get; set; }
        public List<int> PeopleWhoAccepted { get; set; }
        public List<int> PeopleWhoDeclined { get; set; }
        public List<string> TimeList { get; set; }

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
            FoodItems.ForEach(x => dataModel.FoodItems.Add(x.GetDataModel()));
            Games.ForEach(x => dataModel.Games.Add(x.GetDataModel()));
            dataModel.PeopleInvited = new List<Person>();
            dataModel.PeopleWhoAccepted = new List<Person>();
            dataModel.PeopleWhoDeclined = new List<Person>();

            return dataModel;
        }

        #endregion
    }
}