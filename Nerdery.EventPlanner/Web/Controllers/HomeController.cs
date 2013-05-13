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
    public class HomeController : Controller
    {
        private readonly IRepository<Person> _personRepository;

        public HomeController(IRepository<Person> personRepo)
        {
            _personRepository = personRepo;
        }

        #region Public Methods

        public ActionResult Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage = GetMessageFromMessageId(message);

            //Build the view model for the home page
            var currentUser = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == WebSecurity.GetUserId(User.Identity.Name))
            var model = new HomeViewModel(currentUser);

            return View(model);
        }

        #endregion

        #region Helpers

        public enum ManageMessageId
        {
            EventEditSuccess,
        }

        /// <summary>
        /// Get the appropriate message for the specified message id
        /// </summary>
        /// <param name="id">The specified ManageMessageId</param>
        /// <returns></returns>
        private string GetMessageFromMessageId(ManageMessageId? id)
        {
            string message = id == ManageMessageId.EventEditSuccess ? Constants.HOME_EDIT_SUCCESS
                            : "";
            return message;
        }

        #endregion
    }
}
