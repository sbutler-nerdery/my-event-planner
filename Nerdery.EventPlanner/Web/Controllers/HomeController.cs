using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Data;
using Web.Data.Models;
using Web.Extensions;
using Web.Utilities;
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
        private readonly IRepository<FoodItem> _foodRepository;
        private readonly IRepository<Game> _gameRepository;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IEventService _eventService;

        #endregion 

        #region Constructors

        public HomeController(IRepositoryFactory factory, IUserService userService, INotificationService notificationService,
            IEventService eventService)
        {
            _personRepository = factory.GetRepository<Person>();
            _eventRepository = factory.GetRepository<Event>();
            _foodRepository = factory.GetRepository<FoodItem>();
            _gameRepository = factory.GetRepository<Game>();
            _userService = userService;
            _notificationService = notificationService;
            _eventService = eventService;
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

        public ActionResult ViewInvitation(int eventId, int accepteeId)
        {
            ViewBag.StatusMessage = string.Empty;

            try
            {
                //Get the event
                var theEvent = _eventRepository.GetAll().IncludeAll("Coordinator").FirstOrDefault(x => x.EventId == eventId);
                var theAcceptee = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == accepteeId);

                //Build the view model
                var viewModel = GetViewModel(theEvent, theAcceptee);

                //Setup the session manager
                SetupSession(theEvent, theAcceptee);

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
        [ValidateAntiForgeryToken]
        public ActionResult ViewInvitation(InvitationDetailsViewModel model)
        {
            Event theEvent = null;
            Person thePerson = null;
            try
            {
                theEvent = _eventRepository.GetAll().IncludeAll("Coordinator").FirstOrDefault(x => x.EventId == model.EventId);
                thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == model.PersonId);

                if (ModelState.IsValid)
                {
                    PopulateFoodAndGames(theEvent, thePerson, model);

                    //Process food items
                    _eventService.AppendNewFoodItems(theEvent, model);
                    _eventService.RemoveFoodItems(theEvent, model);

                    //Process games
                    _eventService.AppendNewGames(theEvent, model);
                    _eventService.RemoveGames(theEvent, model);

                    _personRepository.SubmitChanges();
                    _eventRepository.SubmitChanges();
                    _foodRepository.SubmitChanges();
                    _eventRepository.SubmitChanges();

                    return RedirectToAction("Index", new { message = BaseControllerMessageId.UpdateInvitationSuccess });
                }                
            }
            catch (Exception)
            {
                //TODO:log to database
            }

            //If we get to here there is a problem
            ViewBag.StatusMessage = GetMessageFromMessageId(BaseControllerMessageId.UpdateInvitationFail);
            model = GetViewModel(theEvent, thePerson);
            return View(model);
        }

        public ActionResult AcceptInvitation(int eventId, int accepteeId)
        {
            ViewBag.StatusMessage = string.Empty;

            try
            {
                //Get the event
                var theEvent = _eventRepository.GetAll().IncludeAll("Coordinator").FirstOrDefault(x => x.EventId == eventId);
                var theAcceptee = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == accepteeId);

                //Build the view model
                var viewModel = GetViewModel(theEvent, theAcceptee);

                //Setup the session manager
                SetupSession(theEvent, theAcceptee);

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
        [ValidateAntiForgeryToken]
        public ActionResult AcceptInvitation(InvitationDetailsViewModel model)
        {
            Event theEvent = null;
            Person thePerson = null;

            try
            {
                theEvent = _eventRepository.GetAll().IncludeAll("Coordinator").FirstOrDefault(x => x.EventId == model.EventId);
                thePerson =
                    _personRepository.GetAll()
                                     .FirstOrDefault(x => x.PersonId == model.PersonId);

                if (ModelState.IsValid)
                {
                    PopulateFoodAndGames(theEvent, thePerson, model);
                    thePerson.AmAttending.Add(theEvent);
                    thePerson.HaveDeclined.Remove(theEvent);

                    //Add the coordinator to this peron's friend list.
                    var exists = thePerson.MyRegisteredFriends.FirstOrDefault(x => x.PersonId == theEvent.Coordinator.PersonId);

                    if (exists == null)
                        thePerson.MyRegisteredFriends.Add(theEvent.Coordinator);

                    //Process food items
                    _eventService.AppendNewFoodItems(theEvent, model);
                    _eventService.RemoveFoodItems(theEvent, model);

                    //Process games
                    _eventService.AppendNewGames(theEvent, model);
                    _eventService.RemoveGames(theEvent, model);

                    _personRepository.SubmitChanges();
                    _eventRepository.SubmitChanges();
                    _foodRepository.SubmitChanges();
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
            }

            //If we get to here there is a problem
            ViewBag.StatusMessage = GetMessageFromMessageId(BaseControllerMessageId.AcceptInvitationFail);
            model = GetViewModel(theEvent, thePerson);
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

        #region Helpers

        private InvitationDetailsViewModel GetViewModel(Event theEvent, Person thePerson)
        {
            var model = new InvitationDetailsViewModel { EventId = theEvent.EventId, 
                Title = theEvent.Title, 
                Description = theEvent.Description, 
                PersonId = thePerson.PersonId, 
                StartDate = theEvent.StartDate,
                StartTime = theEvent.StartDate.ToString("h:mm tt"),
                EndTime = theEvent.EndDate.ToString("h:mm tt"),
                PeopleInvited = new List<PersonViewModel>()
            };

            PopulateFoodAndGames(theEvent, thePerson, model);

            model.Coordinator = new PersonViewModel(theEvent.Coordinator);
            model.EventLocation = theEvent.Location;

            //Stuff the user is already bringing
            var eventFoodItemIds = theEvent.FoodItems.Select(x => x.FoodItemId);
            var personFoodItemIds = thePerson.MyFoodItems.Select(x => x.FoodItemId);
            var selectedFoodItems = personFoodItemIds.Intersect(eventFoodItemIds);
            model.WillBringTheseFoodItems = 
                theEvent.FoodItems.Where(x => selectedFoodItems.Contains(x.FoodItemId)).Select(x => new FoodItemViewModel(x)).ToList();

            //Populate the event id for each item
            model.WillBringTheseFoodItems.ForEach(x => x.EventId = theEvent.EventId);

            var eventGameIds = theEvent.Games.Select(x => x.GameId);
            var personGameIds = thePerson.MyGames.Select(x => x.GameId);
            var selectedGames = personGameIds.Intersect(eventGameIds);
            model.WillBringTheseGames =
                theEvent.Games.Where(x => selectedGames.Contains(x.GameId)).Select(x => new GameViewModel(x)).ToList();

            //Populate the event id for each item
            model.WillBringTheseGames.ForEach(x => x.EventId = theEvent.EventId);

            //People who are coming
            theEvent.RegisteredInvites.ForEach(x => model.PeopleInvited.Add(new PersonViewModel(x)));
            theEvent.UnRegisteredInvites.ForEach(x => model.PeopleInvited.Add(new PersonViewModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName
                }));

            //Order by first name, last name
            model.PeopleInvited = model.PeopleInvited
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName).ToList();

            return model;
        }

        private void PopulateFoodAndGames(Event theEvent, Person thePerson, InvitationDetailsViewModel model)
        {
            //Populate the games and food that are already coming
            theEvent.FoodItems.ForEach(x => model.AllEventFoodItems.Add(new FoodItemViewModel(x)));
            theEvent.Games.ForEach(x => model.AllEventGames.Add(new GameViewModel(x)));

            //Populate all of the food items and games owned by the person
            var foodItems = new List<SelectListItem>();
            var games = new List<SelectListItem>();

            thePerson.MyFoodItems
                             .ToList()
                             .ForEach(
                                 x => foodItems.Add(new SelectListItem
                                 {
                                     Value = x.FoodItemId.ToString(),
                                     Text = x.Title
                                 }));

            thePerson.MyGames
                             .ToList()
                             .ForEach(
                                 x => games.Add(new SelectListItem
                                 {
                                     Value = x.GameId.ToString(),
                                     Text = x.Title
                                 }));

            model.MyFoodItems = new MultiSelectList(foodItems, "Value", "Text");
            model.MyGames = new MultiSelectList(games, "Value", "Text");
        }

        #endregion
    }
}
