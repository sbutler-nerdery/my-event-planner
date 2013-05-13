using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Data;
using Web.Data.Models;
using Web.Services;
using Web.ViewModels;
using WebMatrix.WebData;

namespace Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IUserService _userService;

        public HomeController(IRepository<Person> personRepo, IUserService userService)
        {
            _personRepository = personRepo;
            _userService = userService;
        }

        #region Public Methods

        public ActionResult Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage = GetMessageFromMessageId(message);

            try
            {
                //This is here for unit testing... the assumption is that you would not get this far if the user was not logged in.
                var userName = User != null ? User.Identity.Name : "";
                var currentUser =
                    _personRepository.GetAll().FirstOrDefault(x => x.PersonId == _userService.GetCurrentUserId(userName));
                var model = new HomeViewModel(currentUser);

                return View(model);
            }
            catch (Exception)
            {
                //TODO: log error to database
                ViewBag.StatusMessage = GetMessageFromMessageId(ManageMessageId.BuildViewModelFail);
            }
            //Build the view model for the home page

            return View();
        }

        #endregion

        #region Helpers

        public enum ManageMessageId
        {
            EventEditSuccess,
            BuildViewModelFail
        }

        /// <summary>
        /// Get the appropriate message for the specified message id
        /// </summary>
        /// <param name="id">The specified ManageMessageId</param>
        /// <returns></returns>
        private string GetMessageFromMessageId(ManageMessageId? id)
        {
            string message = id == ManageMessageId.EventEditSuccess ? Constants.HOME_EDIT_SUCCESS
                            : id == ManageMessageId.BuildViewModelFail ? Constants.HOME_BUILD_VIEW_FAIL
                            : "";
            return message;
        }

        #endregion
    }
}
