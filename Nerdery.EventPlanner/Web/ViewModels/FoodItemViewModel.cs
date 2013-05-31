using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Web.Data.Models;

namespace Web.ViewModels
{
    /// <summary>
    /// An item of food or drink that can be taken to an event
    /// </summary>
    public class FoodItemViewModel
    {
        #region Constructors

        public FoodItemViewModel()
        {
        }

        public FoodItemViewModel(FoodItem model)
        {
            FoodItemId = model.FoodItemId;
            Title = model.Title;
            Description = model.Description;
        }

        #endregion 

        #region Properties
        /// <summary>
        /// Get or set the event id that this food item belongs to
        /// </summary>
        public int EventId { get; set; }
        public int FoodItemId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// The index of this item when part of a collection
        /// </summary>
        public int Index { get; set; }

        #endregion 

        #region Methods

        /// <summary>
        /// Get a data model for this food item.
        /// </summary>
        /// <returns></returns>
        public FoodItem GetDataModel()
        {
            var dataModel = new FoodItem();
            dataModel.FoodItemId = FoodItemId;
            dataModel.Title = Title;
            dataModel.Description = Description;

            return dataModel;
        }

        #endregion
    }
}