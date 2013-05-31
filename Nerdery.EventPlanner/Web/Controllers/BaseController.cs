using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Data;
using Web.Data.Models;
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
                            : "";
            return message;
        }

        #endregion
    }
}
