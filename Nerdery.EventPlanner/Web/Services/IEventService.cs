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
        /// Get the event start time by putting together the date values from the first parameter with the time values
        /// of the second parameter
        /// </summary>
        /// <param name="startDate">The start date of the event</param>
        /// <param name="startTime">The start time of the event... the date portion of this parameter will be ignored</param>
        /// <returns></returns>
        DateTime GetEventStartDate(DateTime startDate, DateTime startTime);
        /// <summary>
        /// Get the correct end date value for the start and end date pair provided.
        /// </summary>
        /// <param name="startDate">The specified start date</param>
        /// <param name="endDate">The specified end date to be modified.</param>
        /// <example>start date: 4/4/2012 4:15 PM, end date: 4/4/2012 1:00 AM will return an end date value of 4/5/2012 1:00 AM</example>
        DateTime GetEventEndDate(DateTime startDate, DateTime endDate);
        /// <summary>
        /// Add the appropriate food items to the data model
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        /// <returns></returns>
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
