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
        void SetEventDates(Event dataModel, EditEventViewModel viewModel);
        /// <summary>
        /// Add the appropriate food items to the data model
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        void AppendNewFoodItems(Event dataModel, EditEventViewModel viewModel);
        /// <summary>
        /// Remove the appropriate food items from the event data model.
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        /// <returns></returns>
        void RemoveFoodItems(Event dataModel, EditEventViewModel viewModel);
        /// <summary>
        /// Add the appropriate games to the event data model
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        /// <returns></returns>
        void AppendNewGames(Event dataModel, EditEventViewModel viewModel);
        /// <summary>
        /// Remove the appropriate games from the event data model
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        /// <returns></returns>
        void RemoveGames(Event dataModel, EditEventViewModel viewModel);
        /// <summary>
        /// Invite the appropriate people to the event
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        /// <returns></returns>
        void InviteNewPeople(Event dataModel, EditEventViewModel viewModel);
        /// <summary>
        /// Uninvite the appropriate people from the event
        /// </summary>
        /// <param name="dataModel">The specified event data model</param>
        /// <param name="viewModel">The specified event view model</param>
        /// <returns></returns>
        void UninvitePeople(Event dataModel, EditEventViewModel viewModel);
        /// <summary>
        /// Get a serialized JSON string of the required fields of the event model.
        /// </summary>
        /// <param name="dataModel">An event model</param>
        /// <returns></returns>
        string GetSerializedModelState(Event dataModel);
        /// <summary>
        /// Get the list of invited people that appear in the current list but not in the previous list
        /// </summary>
        /// <param name="previousInvites">The list of previously invited people</param>
        /// <param name="currentInvites">The list of currently invited people</param>
        /// <returns></returns>
        List<Person> GetRegisteredInvites(List<Person> previousInvites, List<Person> currentInvites);
        /// <summary>
        /// Get the list of non-registered invited people that appear in the current list but not in the previous list
        /// </summary>
        /// <param name="previousInvites">The list of previously invited people</param>
        /// <param name="currentInvites">The list of currently invited people</param>
        /// <returns></returns>
        List<PendingInvitation> GetNonRegisteredInvites(List<PendingInvitation> previousInvites, List<PendingInvitation> currentInvites);
        /// <summary>
        /// Get the list of uninvited people that appear in the current list but not in the previous list
        /// </summary>
        /// <param name="previousInvites">The list of previously invited people</param>
        /// <param name="currentInvites">The list of currently invited people</param>
        /// <returns></returns>
        List<Person> GetRegisteredUninvites(List<Person> previousInvites, List<Person> currentInvites);
        /// <summary>
        /// Get the list of non-registered uninvited people that appear in the current list but not in the previous list
        /// </summary>
        /// <param name="previousInvites">The list of previously invited people</param>
        /// <param name="currentInvites">The list of currently invited people</param>
        /// <returns></returns>
        List<PendingInvitation> GetNonRegisteredUninvites(List<PendingInvitation> previousInvites, List<PendingInvitation> currentInvites);
    }
}
