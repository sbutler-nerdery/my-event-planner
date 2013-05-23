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
            CurrentEventFoodItems = new List<FoodItemViewModel>();
            CurrentEventGames = new List<GameViewModel>();
            WillBringTheseFoodItems = new List<string>();
            WillBringTheseGames = new List<string>();
        }
        /// <summary>
        /// Get or set the list of food items that people are already bringing to the event
        /// </summary>
        public List<FoodItemViewModel> CurrentEventFoodItems { get; set; }
        /// <summary>
        /// Get or set the list of games that people are already bringing to the event
        /// </summary>
        public List<GameViewModel> CurrentEventGames { get; set; }
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