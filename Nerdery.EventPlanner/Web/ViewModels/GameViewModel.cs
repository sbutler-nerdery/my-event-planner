using System;
using System.Collections.Generic;
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
        public string Title { get; set; }
        public string Description { get; set; }

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