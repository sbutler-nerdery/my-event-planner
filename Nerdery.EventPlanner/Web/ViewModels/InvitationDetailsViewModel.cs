using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.ViewModels
{
    public class InvitationDetailsViewModel
    {
        public InvitationDetailsViewModel()
        {
            CurrentEventFoodItems = new List<FoodItemViewModel>();
            CurrentEventGames = new List<GameViewModel>();
            WillBringTheseFoodItems = new List<string>();
            WillBringTheseGames = new List<string>();
        }
        /// <summary>
        /// Get or set the event id
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// Get or set the title of the event.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Get or set the description of an event.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Get or set the person id for the user accepting the invitation
        /// </summary>
        public int AccepteeId { get; set; }
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
        /// <summary>
        /// Get or set the list of food item ids that the user will add to the event
        /// </summary>
        public List<string> WillBringTheseFoodItems { get; set; }
        /// <summary>
        /// Get or set the list of games ids that the user will bring ot the event
        /// </summary>
        public List<string> WillBringTheseGames { get; set; }
        public FoodItemViewModel AddFoodItem { get; set; }
        public GameViewModel AddGameItem { get; set; }
    }
}