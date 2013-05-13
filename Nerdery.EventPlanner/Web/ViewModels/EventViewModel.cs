using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
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
            PeopleList = new List<PersonViewModel>();
            PeopleInvited = new List<PersonViewModel>();
            PeopleWhoAccepted = new List<PersonViewModel>();
            PeopleWhoDeclined = new List<PersonViewModel>();
        }

        public EventViewModel(Event model) :this()
        {
            EventId = model.EventId;
            Title = model.Title;
            Description = model.Description;
            Location = model.Location;
            Coordinator = new PersonViewModel(model.Coordinator);
            StartDate = model.StartDate;
            StartTime = model.StartDate;
            EndTime = model.EndDate;
            model.FoodItems.ForEach(x => FoodItems.Add(new FoodItemViewModel(x)));
            model.Games.ForEach(x => Games.Add(new GameViewModel(x)));
            model.PeopleInvited.ForEach(x => PeopleInvited.Add(new PersonViewModel(x)));
            model.PeopleWhoAccepted.ForEach(x => PeopleWhoAccepted.Add(new PersonViewModel(x)));
            model.PeopleWhoDeclined.ForEach(x => PeopleWhoDeclined.Add(new PersonViewModel(x)));
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
        /// <summary>
        /// Get or set the complete list of people who could be invited to an event
        /// </summary>
        public List<PersonViewModel> PeopleList { get; set; }
        public List<PersonViewModel> PeopleInvited { get; set; }
        public List<PersonViewModel> PeopleWhoAccepted { get; set; }
        public List<PersonViewModel> PeopleWhoDeclined { get; set; }

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
            dataModel.Coordinator = Coordinator.GetDataModel();
            dataModel.PeopleInvited = new List<Person>();
            dataModel.FoodItems = new List<FoodItem>();
            dataModel.Games = new List<Game>();
            FoodItems.ForEach(x => dataModel.FoodItems.Add(x.GetDataModel()));
            Games.ForEach(x => dataModel.Games.Add(x.GetDataModel()));

            return dataModel;
        }

        #endregion
    }
}