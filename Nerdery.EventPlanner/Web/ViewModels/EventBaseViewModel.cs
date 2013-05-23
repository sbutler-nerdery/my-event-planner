using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.ViewModels
{
    public class EventBaseViewModel
    {
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
        /// Get or set the list of food item ids that the user will add to the event
        /// </summary>
        public List<string> WillBringTheseFoodItems { get; set; }
        /// <summary>
        /// Get or set the list of games ids that the user will bring ot the event
        /// </summary>
        public List<string> WillBringTheseGames { get; set; }
        public FoodItemViewModel AddFoodItem { get; set; }
        public GameViewModel AddGameItem { get; set; }
        /// <summary>
        /// Get or set the person id for the user accepting the invitation
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// Get or set the control id for the listbox that will be updated when adding food items
        /// </summary>
        public string FoodControlId { get; set; }
        /// <summary>
        /// Get or set the control id for the listbox that will be updated when adding game items
        /// </summary>
        public string GameControlId { get; set; }
        /// <summary>
        /// Get or set a list of all the food items being brought to the event
        /// </summary>
        [Display(Name = "Food items coming to the event")]
        public List<FoodItemViewModel> AllEventFoodItems { get; set; }
        /// <summary>
        /// Get or set a list of all the games being brought to the event
        /// </summary>
        [Display(Name = "Games coming to the event")]
        public List<GameViewModel> AllEventGames { get; set; }
    }
}