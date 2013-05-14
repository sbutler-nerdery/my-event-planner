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
        private readonly IRepository<Event> _eventRepository;
        private readonly IUserService _userService;

        #endregion 

        #region Constructors

        public HomeController(IRepository<Person> personRepo, IRepository<Event> eventRepo, IUserService userService)
        {
            _personRepository = personRepo;
            _eventRepository = eventRepo;
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

        public ActionResult AcceptInvitation(int eventId, int accepteeId)
        {
            ViewBag.StatusMessage = string.Empty;

            try
            {
                //Get the event
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
                var theAcceptee = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == accepteeId);

                //Build the view model
                var viewModel = new AcceptInvitationViewModel{EventId = eventId, AccepteeId = accepteeId};
                theEvent.FoodItems.ForEach(x => viewModel.CurrentEventFoodItems.Add(new FoodItemViewModel(x)));
                theEvent.Games.ForEach(x => viewModel.CurrentEventGames.Add(new GameViewModel(x)));
                theAcceptee.MyFoodItems.ForEach(x => viewModel.AccepteeFoodItems.Add(new FoodItemViewModel(x)));
                theAcceptee.MyGames.ForEach(x => viewModel.AccepteeGames.Add(new GameViewModel(x)));

                return View(viewModel);
            }
            catch (Exception)
            {
                //TODO:log to database
                ViewBag.StatusMessage = GetMessageFromMessageId(BaseControllerMessageId.BuildViewModelFail);
            }

            return View();
        }

        [HttpPost]
        public ActionResult AcceptInvitation(AcceptInvitationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == model.EventId);
                    var thePerson =
                        _personRepository.GetAll()
                                         .FirstOrDefault(x => x.PersonId == model.AccepteeId);

                    thePerson.AmAttending.Add(theEvent);
                    thePerson.HaveDeclined.Remove(theEvent);

                    model.WillBringTheseFoodItems.ForEach(x => theEvent.FoodItems.Add(x.GetDataModel()));
                    model.WillBringTheseGames.ForEach(x => theEvent.Games.Add(x.GetDataModel()));

                    _personRepository.SubmitChanges();
                    _eventRepository.SubmitChanges();

                    return RedirectToAction("Index", new { message = BaseControllerMessageId.AcceptInvitationSuccess });
                }                
            }
            catch (Exception)
            {
                //TODO:log to database
                ViewBag.StatusMessage = GetMessageFromMessageId(BaseControllerMessageId.BuildViewModelFail);
            }

            //If we get to here there is a problem
            return View(model);
        }

        #endregion
    }
}
