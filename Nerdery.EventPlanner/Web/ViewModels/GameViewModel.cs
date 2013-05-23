using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Web.Data.Models;

namespace Web.ViewModels
{
    /// <summary>
    /// A game that can be taken to an event
    /// </summary>
    public class GameViewModel
    {
        #region Constructors

        public GameViewModel()
        {
        }

        public GameViewModel(Game model)
        {
            GameId = model.GameId;
            Title = model.Title;
            Description = model.Description;
        }

        #endregion 

        #region Properties

        public int GameId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Get or set the control id for the listbox that will be updated when adding game items
        /// </summary>
        public string GameControlId { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Get a data model for this game.
        /// </summary>
        /// <returns></returns>
        public Game GetDataModel()
        {
            var dataModel = new Game();
            dataModel.GameId = GameId;
            dataModel.Title = Title;
            dataModel.Description = Description;

            return dataModel;
        }

        #endregion
    }
}