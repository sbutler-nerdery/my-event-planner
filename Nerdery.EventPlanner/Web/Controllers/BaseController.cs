using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (!WebSecurity.Initialized)
                WebSecurity.InitializeDatabaseConnection(Constants.DB_CONNECTION_STRING,
                    Constants.DB_USER_TABLE_NAME,
                    Constants.DB_USER_ID_COLUMN,
                    Constants.DB_USER_NAME_COLUMN, autoCreateTables: true);
        }

        #region Helpers

        public enum BaseControllerMessageId
        {
            SaveModelSuccess,
            SaveModelFailed,
            DeleteSuccessful,
            DeleteFailed,
            BuildViewModelFail,
            AcceptInvitationFail,
            AcceptInvitationSuccess
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
                            : "";
            return message;
        }

        #endregion
    }
}
