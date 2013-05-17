using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Data.Models;
using Web.ViewModels;

namespace Web.Services
{
    /// <summary>
    /// This is a service contract used for performing tasks against the event data and view models
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Get a list of AM / PM friendly times for a 12 hour period
        /// </summary>
        /// <returns></returns>
        List<string> GetTimeList();
        /// <summary>
        /// Set the event start date and end date for the data model
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        void SetEventDates(Event dataModel, EventViewModel viewModel);
        /// <summary>
        /// Add the appropriate food items to the data model
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        void AppendNewFoodItems(Event dataModel, EventViewModel viewModel);
        /// <summary>
        /// Remove the appropriate food items from the event data model.
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        /// <returns></returns>
        void RemoveFoodItems(Event dataModel, EventViewModel viewModel);
        /// <summary>
        /// Add the appropriate games to the event data model
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        /// <returns></returns>
        void AppendNewGames(Event dataModel, EventViewModel viewModel);
        /// <summary>
        /// Remove the appropriate games from the event data model
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        /// <returns></returns>
        void RemoveGames(Event dataModel, EventViewModel viewModel);
        /// <summary>
        /// Invite the appropriate people to the event
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        /// <returns></returns>
        void InviteNewPeople(Event dataModel, EventViewModel viewModel);
        /// <summary>
        /// Uninvite the appropriate people from the event
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        /// <returns></returns>
        void UninvitePeople(Event dataModel, EventViewModel viewModel);
    }
}
