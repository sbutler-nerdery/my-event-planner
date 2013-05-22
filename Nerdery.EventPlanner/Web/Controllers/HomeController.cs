using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Data;
using Web.Data.Models;
using Web.Extensions;
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
        private readonly INotificationService _notificationService;

        #endregion 

        #region Constructors

        public HomeController(IRepository<Person> personRepo, IRepository<Event> eventRepo, IUserService userService, INotificationService notificationService)
        {
            _personRepository = personRepo;
            _eventRepository = eventRepo;
            _userService = userService;
            _notificationService = notificationService;
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
                var userId = _userService.GetCurrentUserId(userName);
                var currentUser =
                    _personRepository.GetAll().FirstOrDefault(x => x.PersonId == userId);

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
                var viewModel = new AcceptInvitationViewModel{EventId = eventId, Title = theEvent.Title, Description = theEvent.Description, AccepteeId = accepteeId};
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
                    var theEvent = _eventRepository.GetAll().IncludeAll("Coordinator").FirstOrDefault(x => x.EventId == model.EventId);
                    var thePerson =
                        _personRepository.GetAll()
                                         .FirstOrDefault(x => x.PersonId == model.AccepteeId);

                    thePerson.AmAttending.Add(theEvent);
                    thePerson.HaveDeclined.Remove(theEvent);
                    
                    //Add the coordinator to this peron's friend list.
                    var exists = thePerson.MyRegisteredFriends.FirstOrDefault(x => x.PersonId == theEvent.Coordinator.PersonId);

                    if (exists == null)
                        thePerson.MyRegisteredFriends.Add(theEvent.Coordinator);

                    model.WillBringTheseFoodItems.ForEach(x => theEvent.FoodItems.Add(x.GetDataModel()));
                    model.WillBringTheseGames.ForEach(x => theEvent.Games.Add(x.GetDataModel()));

                    _personRepository.SubmitChanges();
                    _eventRepository.SubmitChanges();

                    //Send notification to event coordinator
                    var notification = _notificationService.GetInvitationAcceptedNotification(theEvent.EventId, thePerson.PersonId);
                    _notificationService.SendNotifications(new List<EventPlannerNotification> { notification });

                    return RedirectToAction("Index", new { message = BaseControllerMessageId.AcceptInvitationSuccess });
                }                
            }
            catch (Exception)
            {
                //TODO:log to database
                ViewBag.StatusMessage = GetMessageFromMessageId(BaseControllerMessageId.AcceptInvitationFail);
            }

            //If we get to here there is a problem
            return View(model);
        }

        /// <summary>
        /// Decline an invitation to an event
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="accepteeId">The specified user id</param>
        /// <returns></returns>
        public ActionResult DeclineInviation(int eventId, int accepteeId)
        {
            try
            {
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
                var thePerson =
                    _personRepository.GetAll()
                                     .FirstOrDefault(x => x.PersonId == accepteeId);

                thePerson.AmAttending.Remove(theEvent);
                thePerson.HaveDeclined.Add(theEvent);
                _personRepository.SubmitChanges();

                //Send notification to event coordinator
                var notification = _notificationService.GetInvitationDeclinedNotification(eventId, accepteeId);
                _notificationService.SendNotifications(new List<EventPlannerNotification> { notification });
            }
            catch (Exception)
            {
                //TODO: log error to database
                return RedirectToAction("Index", new {message = BaseControllerMessageId.DeclineInvitationFail});
            }

            return RedirectToAction("Index", new { message = BaseControllerMessageId.DeclineInvitationSuccess });
        }

        #endregion
    }
}
