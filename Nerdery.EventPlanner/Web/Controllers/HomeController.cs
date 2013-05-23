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
        private readonly IRepository<FoodItem> _foodRepository;
        private readonly IRepository<Game> _gameRepository;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        #endregion 

        #region Constructors

        public HomeController(IRepository<Person> personRepo, IRepository<Event> eventRepo, IRepository<FoodItem> foodRepo,
            IRepository<Game> gameRepo, IUserService userService, INotificationService notificationService)
        {
            _personRepository = personRepo;
            _eventRepository = eventRepo;
            _foodRepository = foodRepo;
            _gameRepository = gameRepo;
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

        public ActionResult ViewInvitation(int eventId, int accepteeId)
        {
            ViewBag.StatusMessage = string.Empty;

            try
            {
                //Get the event
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
                var theAcceptee = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == accepteeId);

                //Build the view model
                var viewModel = GetViewModel(theEvent, theAcceptee); 

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
        public ActionResult ViewInvitation(InvitationDetailsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var theEvent = _eventRepository.GetAll().IncludeAll("Coordinator").FirstOrDefault(x => x.EventId == model.EventId);
                    var thePerson =
                        _personRepository.GetAll()
                                         .FirstOrDefault(x => x.PersonId == model.AccepteeId);

                    ProcessFoodAndGameUpdates(model, theEvent);

                    _personRepository.SubmitChanges();
                    _eventRepository.SubmitChanges();
                    _foodRepository.SubmitChanges();
                    _eventRepository.SubmitChanges();

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

        public ActionResult AcceptInvitation(int eventId, int accepteeId)
        {
            ViewBag.StatusMessage = string.Empty;

            try
            {
                //Get the event
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
                var theAcceptee = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == accepteeId);

                //Build the view model
                var viewModel = GetViewModel(theEvent, theAcceptee); 

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
        public ActionResult AcceptInvitation(InvitationDetailsViewModel model)
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

                    ProcessFoodAndGameUpdates(model, theEvent);

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

        #region Helpers

        private void ProcessFoodAndGameUpdates(InvitationDetailsViewModel model, Event theEvent)
        {
            //Get a list of the new food items
            var dataFoodIds = theEvent.FoodItems.Select(x => x.FoodItemId).ToArray(); //Items in the database
            var newFoodItems = model.WillBringTheseFoodItems.Select(int.Parse).Where(x => !dataFoodIds.Contains(x)).ToList();

            //Get list of removed food items
            var modelFoodIds = model.WillBringTheseFoodItems.Select(int.Parse).ToArray(); //Items in local view model
            var deletedFoodItemIds = theEvent.FoodItems.Where(x => !modelFoodIds.Contains(x.FoodItemId)).Select(x => x.FoodItemId).ToList();

            //Add food items
            newFoodItems.ForEach(foodId =>
            {
                var addMe = _foodRepository.GetAll().FirstOrDefault(y => y.FoodItemId == foodId);

                if (addMe != null)
                    theEvent.FoodItems.Add(addMe);
            });

            //Remove games
            deletedFoodItemIds.ForEach(foodId =>
            {
                var removeMe = _foodRepository.GetAll().FirstOrDefault(y => y.FoodItemId == foodId);

                if (removeMe != null)
                    theEvent.FoodItems.Remove(removeMe);
            });

            //Get a list of the new food items
            var dataGameIds = theEvent.Games.Select(x => x.GameId).ToArray(); //Items in the database
            var newGames = model.WillBringTheseGames.Select(int.Parse).Where(x => !dataGameIds.Contains(x)).ToList();

            //Get list of removed food items
            var modelGameIds = model.WillBringTheseGames.Select(int.Parse).ToArray(); //Items in local view model
            var deletedGameIds = theEvent.Games.Where(x => !modelGameIds.Contains(x.GameId)).Select(x => x.GameId).ToList();

            //Add games
            newGames.ForEach(ganeId =>
            {
                var addMe = _gameRepository.GetAll().FirstOrDefault(y => y.GameId == ganeId);

                if (addMe != null)
                    theEvent.Games.Add(addMe);
            });

            //Remove games
            deletedGameIds.ForEach(gameId =>
            {
                var removeMe = _gameRepository.GetAll().FirstOrDefault(y => y.GameId == gameId);

                if (removeMe != null)
                    theEvent.Games.Remove(removeMe);
            });
        }

        private InvitationDetailsViewModel GetViewModel(Event theEvent, Person thePerson)
        {
            var model = new InvitationDetailsViewModel { EventId = theEvent.EventId, 
                Title = theEvent.Title, 
                Description = theEvent.Description, 
                AccepteeId = thePerson.PersonId 
            };

            //Populate the games and food that are already coming
            theEvent.FoodItems.ForEach(x => model.CurrentEventFoodItems.Add(new FoodItemViewModel(x)));
            theEvent.Games.ForEach(x => model.CurrentEventGames.Add(new GameViewModel(x)));

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

            //Stuff the user is already bringing
            var eventFoodItemIds = theEvent.FoodItems.Select(x => x.FoodItemId).ToList();
            var personFoodItemIds = thePerson.MyFoodItems.Select(x => x.FoodItemId).ToList();
            model.WillBringTheseFoodItems = personFoodItemIds.Intersect(eventFoodItemIds).Select(x => x.ToString()).ToList();

            var eventGameIds = theEvent.Games.Select(x => x.GameId).ToList();
            var personGameIds = thePerson.MyGames.Select(x => x.GameId).ToList();
            model.WillBringTheseGames = personGameIds.Intersect(eventGameIds).Select(x => x.ToString()).ToList();

            return model;
        }

        #endregion
    }
}
