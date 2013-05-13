using System;
using System.Collections.Generic;
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

        public int FoodItemId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

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