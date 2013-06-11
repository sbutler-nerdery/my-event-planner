using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.ViewModels
{
    public class InvitationDetailsViewModel : EventBaseViewModel
    {
        public InvitationDetailsViewModel()
        {
            PeopleInvited = new List<PersonViewModel>();
            AllEventFoodItems = new List<FoodItemViewModel>();
            AllEventGames = new List<GameViewModel>();
            WillBringTheseFoodItems = new List<FoodItemViewModel>();
            WillBringTheseGames = new List<GameViewModel>();
        }
        /// <summary>
        /// Get or set the event location
        /// </summary>
        public string EventLocation { get; set; }
        /// <summary>
        /// Get or set the coordinator for this event
        /// </summary>
        public PersonViewModel Coordinator { get; set; }
        /// <summary>
        /// Get or set the list of food items belonging to the user accepting the invitation
        /// </summary>
        public MultiSelectList MyFoodItems { get; set; }
        /// <summary>
        /// Get or set the list of games belonging to the user accepting the invitation
        /// </summary>
        public MultiSelectList MyGames { get; set; }
    }
}