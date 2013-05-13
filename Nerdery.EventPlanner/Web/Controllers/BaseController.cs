using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class BaseController : Controller
    {
        #region Helpers

        public enum BaseControllerMessageId
        {
            SaveModelSuccess,
            SaveModelFailed,
            DeleteSuccessful,
            DeleteFailed,
            BuildViewModelFail
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
                            : id == BaseControllerMessageId.SaveModelFailed ? Constants.BASE_SAVE_FAIL
                            : id == BaseControllerMessageId.BuildViewModelFail ? Constants.BASE_BUILD_VIEW_FAIL
                            : "";
            return message;
        }

        #endregion

    }
}
