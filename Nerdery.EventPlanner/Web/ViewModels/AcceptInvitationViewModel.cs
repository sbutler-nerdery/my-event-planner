using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ViewModels
{
    public class AcceptInvitationViewModel
    {
        public AcceptInvitationViewModel()
        {
            CurrentEventFoodItems = new List<FoodItemViewModel>();
            CurrentEventGames = new List<GameViewModel>();
            WillBringTheseFoodItems = new List<FoodItemViewModel>();
            WillBringTheseGames = new List<GameViewModel>();
            AccepteeFoodItems = new List<FoodItemViewModel>();
            AccepteeGames = new List<GameViewModel>();
        }
        /// <summary>
        /// Get or set the event id
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// Get or set the person id for the user accepting the invitation
        /// </summary>
        public int AccepteeId { get; set; }
        /// <summary>
        /// Get or set the list of food items that people are already bringing to the event
        /// </summary>
        public List<FoodItemViewModel> CurrentEventFoodItems { get; set; }
        /// <summary>
        /// Get or set the list of tames that people are already bringing to the event
        /// </summary>
        public List<GameViewModel> CurrentEventGames { get; set; }
        /// <summary>
        /// Get or set the list of food items belonging to the user accepting the invitation
        /// </summary>
        public List<FoodItemViewModel> AccepteeFoodItems { get; set; }
        /// <summary>
        /// Get or set the list of games belonging to the user accepting the invitation
        /// </summary>
        public List<GameViewModel> AccepteeGames { get; set; }
        /// <summary>
        /// Get or set the list of food items that the user will add to the event
        /// </summary>
        public List<FoodItemViewModel> WillBringTheseFoodItems { get; set; }
        /// <summary>
        /// Get or set the list of games that the user will bring ot the event
        /// </summary>
        public List<GameViewModel> WillBringTheseGames { get; set; }
    }
}