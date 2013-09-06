using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Data;
using Web.Data.Models;
using Web.Utilities;
using Web.ViewModels;
using WebMatrix.WebData;

namespace Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        #region Properties

        /// <summary>
        /// Get or set the Facebook access token available to the entire site.
        /// </summary>
        public string AccessToken { get; set; }

        #endregion

        #region Methods

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (!WebSecurity.Initialized)
                WebSecurity.InitializeDatabaseConnection(Constants.DB_CONNECTION_STRING,
                    Constants.DB_USER_TABLE_NAME,
                    Constants.DB_USER_ID_COLUMN,
                    Constants.DB_USER_NAME_COLUMN, autoCreateTables: true);
        }

        public void SetupSession(Event theEvent, Person thePerson)
        {
            //This is for unit testing... the http context will be null, so we skip the rest of this method
            if (System.Web.HttpContext.Current == null)
                return;

            //Reset the session manager
            SessionUtility.Events.Reset(theEvent.EventId);
            SessionUtility.Person.Reset(thePerson.PersonId);

            //Build out the list of food and games items in session
            theEvent.FoodItems.ForEach(x => SessionUtility.Events.AddFoodItem(x.FoodItemId, theEvent.EventId));
            theEvent.Games.ForEach(x => SessionUtility.Events.AddGame(x.GameId, theEvent.EventId));
            thePerson.MyFoodItems.ForEach(x => SessionUtility.Person.AddFoodItem(x.FoodItemId, thePerson.PersonId));
            thePerson.MyGames.ForEach(x => SessionUtility.Person.AddGame(x.GameId, thePerson.PersonId));
            theEvent.RegisteredInvites.ForEach(x => SessionUtility.Events.AddGuest(x.PersonId, theEvent.EventId));
            theEvent.UnRegisteredInvites.ForEach(x => SessionUtility.Events.AddGuest(-x.PendingInvitationId, theEvent.EventId)); //make sure that the id is negative!
        }

        #endregion

        #region Helpers

        public enum BaseControllerMessageId
        {
            SaveModelSuccess,
            SaveModelFailed,
            DeleteSuccessful,
            DeleteFailed,
            BuildViewModelFail,
            AcceptInvitationFail,
            AcceptInvitationSuccess,
            DeclineInvitationFail,
            DeclineInvitationSuccess,
            UpdateInvitationFail,
            UpdateInvitationSuccess
        }

        /// <summary>
        /// Get the appropriate message for the specified message id
        /// </summary>
        /// <param name="id">The specified ManageMessageId</param>
        /// <returns></returns>
        protected string GetMessageFromMessageId(BaseControllerMessageId? id)
        {
            string message = id == BaseControllerMessageId.SaveModelSuccess ? Constants.BASE_SAVE_SUCCESS
                            : id == BaseControllerMessageId.SaveModelFailed ? Constants.BASE_SAVE_FAIL
                            : id == BaseControllerMessageId.BuildViewModelFail ? Constants.BASE_BUILD_VIEW_FAIL
                            : id == BaseControllerMessageId.AcceptInvitationFail ? Constants.BASE_ACCEPT_INVITATION_FAIL
                            : id == BaseControllerMessageId.AcceptInvitationSuccess ? Constants.BASE_ACCEPT_INVITATION_SUCCESS
                            : id == BaseControllerMessageId.UpdateInvitationFail ? Constants.BASE_UPDATE_INVITATION_FAIL
                            : id == BaseControllerMessageId.UpdateInvitationSuccess ? Constants.BASE_UPDATE_INVITATION_SUCCESS
                            : id == BaseControllerMessageId.DeclineInvitationFail ? Constants.BASE_DECLINE_INVITATION_FAIL
                            : id == BaseControllerMessageId.DeclineInvitationSuccess ? Constants.BASE_DECLINE_INVITATION_SUCCESS
                            : id == BaseControllerMessageId.DeleteFailed ? Constants.BASE_DELETE_FAIL
                            : id == BaseControllerMessageId.DeleteSuccessful ? Constants.BASE_DELETE_SUCCESS
                            : "";
            return message;
        }

        #endregion
    }
}
