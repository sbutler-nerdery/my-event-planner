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
    public class HomeController : BaseController
    {
        #region Fields

        private readonly IRepository<Person> _personRepository;
        private readonly IUserService _userService;

        #endregion 

        #region Constructors

        public HomeController(IRepository<Person> personRepo, IUserService userService)
        {
            _personRepository = personRepo;
            _userService = userService;
        }

        #endregion

        #region Public Methods

        public ActionResult Index(BaseControllerMessageId? message)
        {
            ViewBag.StatusMessage = GetMessageFromMessageId(message);

            try
            {
                //This is here for unit testing... the assumption is that you would not get this far if the user was not logged in.
                var userName = User != null ? User.Identity.Name : "";
                var currentUser =
                    _personRepository.GetAll().FirstOrDefault(x => x.PersonId == _userService.GetCurrentUserId(userName));

                //Build the view model for the home page
                var model = new HomeViewModel(currentUser);

                return View(model);
            }
            catch (Exception)
            {
                //TODO: log error to database
                ViewBag.StatusMessage = GetMessageFromMessageId(BaseControllerMessageId.BuildViewModelFail);
            }

            //If it makes it here, something is wrong
            return View();
        }

        #endregion
    }
}
