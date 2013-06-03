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
        [Required]
        public string Title { get; set; }
        /// <summary>
        /// Get or set the description of an event.
        /// </summary>
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? StartDate { get; set; }
        [Required]
        [Display(Name = "Start time")]
        [RegularExpression(@"^([0-1][0-9]:[0-5][0-9]\s[AaPp][Mm])|([1-9]:[0-5][0-9]\s[AaPp][Mm])$", ErrorMessage = "Must enter hh:mm AM or PM")]
        public string StartTime { get; set; }
        [Required]
        [Display(Name = "End time")]
        [RegularExpression(@"^([0-1][0-9]:[0-5][0-9]\s[AaPp][Mm])|([1-9]:[0-5][0-9]\s[AaPp][Mm])$", ErrorMessage = "Must enter hh:mm AM or PM")]
        public string EndTime { get; set; }
        /// <summary>
        /// Get or set the list of food item ids that the user will add to the event
        /// </summary>
        [Display(Name = "Food provided by me")]
        public List<FoodItemViewModel> WillBringTheseFoodItems { get; set; }
        /// <summary>
        /// Get or set the list of games ids that the user will bring ot the event
        /// </summary>
        [Display(Name = "Games provided by me")]
        public List<GameViewModel> WillBringTheseGames { get; set; }
        public FoodItemViewModel AddFoodItem { get; set; }
        public GameViewModel AddGameItem { get; set; }
        public FoodItemViewModel UpdateFoodItem { get; set; }
        public GameViewModel UpdateGameItem { get; set; }
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
        [Display(Name = "Food items other people are bringing")]
        public List<FoodItemViewModel> AllEventFoodItems { get; set; }
        /// <summary>
        /// Get or set a list of all the games being brought to the event
        /// </summary>
        [Display(Name = "Games other people are brining")]
        public List<GameViewModel> AllEventGames { get; set; }
    }
}