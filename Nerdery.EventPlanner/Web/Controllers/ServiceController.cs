using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Web.Data;
using Web.Data.Models;
using Web.Extensions;
using Web.Helpers;
using Web.Services;
using Web.ViewModels;
using WebMatrix.WebData;

namespace Web.Controllers
{
    /// <summary>
    /// This controller is used exclusively for AJAX calls. All of the methods
    /// return JSON objects.
    /// </summary>
    public class ServiceController : BaseController
    {
        #region Fields

        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<PendingInvitation> _inviteRepository;
        private readonly IRepository<FoodItem> _foodRepository;
        private readonly IRepository<Game> _gameRepository;
        private readonly IUserService _userService;
        private readonly IEventService _eventService;

        #endregion

        #region Constructor

        public ServiceController(IRepositoryFactory factory, IUserService userService, IEventService eventService)
        {
            _eventRepository = factory.GetRepository<Event>();
            _personRepository = factory.GetRepository<Person>();
            _foodRepository = factory.GetRepository<FoodItem>();
            _gameRepository = factory.GetRepository<Game>();
            _inviteRepository = factory.GetRepository<PendingInvitation>();
            _userService = userService;
            _eventService = eventService;
        }

        #endregion

        #region Methods

        #region Users
        /// <summary>
        /// Get a list of all friends belonging to the current user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetGuestList(int eventId, string contains)
        {
            var response = new Response { Error = false };

            try
            {
                //Get list of pending food ids for this event from session
                var pendingGuestListIds = SessionHelper.Events.GetGuestList(eventId);
                var personFriendIds = new List<int>();
                var thePerson = GetCurrentUser();

                thePerson.MyRegisteredFriends.ForEach(x => personFriendIds.Add(x.PersonId));
                //Make the unregistered ids negative to avoid conflicts
                thePerson.MyUnRegisteredFriends.ForEach(x => personFriendIds.Add(-x.PendingInvitationId));

                var guestList = GetNonSelectedGuests(pendingGuestListIds, personFriendIds, eventId)
                    .Where(x => 
                        x.FirstName.ToLower().Contains(contains.ToLower()) ||
                        x.LastName.ToLower().Contains(contains.ToLower()));

                response.Data = guestList;
            }
            catch (Exception)
            {
                //TODO: log to database
                response.Error = true;
                response.Message = "An error occurred. Unable to retrieve your list of food items.";
            }

            return Json(response);
        }
        /// <summary>
        /// Get a single guest from the database using the specified id
        /// </summary>
        /// <param name="guestId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEventGuest(int guestId)
        {
            var response = new Response { Error = false };

            try
            {
                var registeredGuest = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == guestId);

                //Change the guestId to a posative value
                guestId = Math.Abs(guestId);
                var unRegisteredGuest = _inviteRepository.GetAll().FirstOrDefault(x => x.PendingInvitationId == guestId);

                var guest = (registeredGuest != null) ? new PersonViewModel(registeredGuest) : new PersonViewModel
                    {
                        PersonId = unRegisteredGuest.PendingInvitationId, 
                        FirstName = unRegisteredGuest.FirstName,
                        LastName = unRegisteredGuest.LastName,
                        Email = unRegisteredGuest.Email,
                        IsRegistered = false
                    };

                response.Data = guest;
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_GET_GUEST_FAIL;
            }

            return Json(response);
        }
        /// <summary>
        /// Add a guest to the user's list of un-registred friends
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddNewGuest(EditEventViewModel model)
        {
            var response = new Response { Error = false };

            try
            {
                //Get the event
                var theEvent = GetEventById(model.EventId);

                //Get the host
                var thePerson = GetPersonById(model.PersonId);

                //Find out the if the user being added already has en email as a registered user
                var exists = _personRepository.GetAll().FirstOrDefault(x => x.Email == model.EmailInvite.Email);

                if (exists == null)
                {
                    var userName = model.EmailInvite.FirstName + " " + model.EmailInvite.LastName;
                    response.Data = new { PersonId = 0, model.EmailInvite.Email, UserName = userName, model.EmailInvite.FirstName, model.EmailInvite.LastName, model.EmailInvite.InviteControlId };

                    //Add the unregistered guest to the database
                    var newGuest = new PendingInvitation
                    {
                        FirstName = model.EmailInvite.FirstName,
                        LastName = model.EmailInvite.LastName,
                        Email = model.EmailInvite.Email
                    };

                    _inviteRepository.Insert(newGuest);
                    _inviteRepository.SubmitChanges();

                    //Add to the event host's list of unregistered friends
                    thePerson.MyUnRegisteredFriends.Add(newGuest);

                    //Add a negative value to the list
                    var tempId = -newGuest.PendingInvitationId;

                    //Add the unregistered user to session
                    SessionHelper.Events.AddGuest(tempId, model.EventId);
                }
                else
                {
                    //Add the registered user to the session
                    SessionHelper.Events.AddGuest(exists.PersonId, model.EventId);
                }

                //Get list of pending invitation ids
                var pendingEventInvitations = SessionHelper.Events.GetGuestList(model.EventId);
                var personFriendsList = GetPersonFriendList(thePerson);

                //Populate the guest list
                var viewModel = GetEventViewModel(theEvent);
                viewModel.PeopleInvited = GetSelectedGuests(pendingEventInvitations, personFriendsList, model.EventId);

                response.Data = RenderRazorViewToString("_InvitedPeopleTemplate", viewModel);

                //Save to the database if no errors have occurred
                _personRepository.SubmitChanges();
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_ADD_GUEST_FAIL;
            }

            return Json(response);
        }
        /// <summary>
        /// Add a guest to an event
        /// </summary>
        /// <param name="guestId"></param>
        /// <param name="eventId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddPreviousGuest(int guestId, int eventId, int personId)
        {
            var response = new Response { Error = false };

            try
            {
                //Get the event
                var theEvent = GetEventById(eventId);

                //Get the event host
                var thePerson = GetPersonById(personId);

                //Add the pending food item to session
                SessionHelper.Events.AddGuest(guestId, eventId);

                //Get list of pending invitation ids
                var pendingEventInvitations = SessionHelper.Events.GetGuestList(eventId);
                var personFriendsList = GetPersonFriendList(thePerson);

                //Populate the guset list
                var viewModel = GetEventViewModel(theEvent);
                viewModel.PeopleInvited = GetSelectedGuests(pendingEventInvitations, personFriendsList, eventId);

                response.Data = RenderRazorViewToString("_InvitedPeopleTemplate", viewModel);
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_ADD_GUEST_FAIL;
            }

            return Json(response);
        }
        /// <summary>
        /// Update a unregistered guest's info.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateGuestInfo(EditEventViewModel model)
        {
            var response = new Response { Error = false };

            try
            {
                //Get the person
                var personId = _userService.GetCurrentUserId(User.Identity.Name);
                var thePerson = GetCurrentUser();

                //Get the event
                var theEvent = GetEventById(model.EventId);

                //Update the food item
                int guestId = Math.Abs(model.UpdateGuest.PersonId); //Make the person id posative
                var updateMe = _inviteRepository.GetAll().FirstOrDefault(x => x.PendingInvitationId == guestId);
                updateMe.FirstName = model.UpdateGuest.FirstName;
                updateMe.LastName = model.UpdateGuest.LastName;
                updateMe.Email = model.UpdateGuest.Email;

                //Get list of pending invitation ids
                var pendingEventInvitations = SessionHelper.Events.GetGuestList(model.EventId);
                var personFriendsList = GetPersonFriendList(thePerson);

                //Populate the guset list
                var viewModel = GetEventViewModel(theEvent);
                viewModel.PeopleInvited = GetSelectedGuests(pendingEventInvitations, personFriendsList, model.EventId);          

                response.Data = RenderRazorViewToString("_InvitedPeopleTemplate", viewModel);

                //Save to the database last
                _inviteRepository.SubmitChanges();
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_UPDATE_GUEST_FAIL;
            }

            return Json(response);
        }
        /// <summary>
        /// Remove a guest from an event
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="guestId">The id of the food item to be removed</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveGuest(int eventId, int guestId)
        {
            var response = new Response { Error = false };

            try
            {
                //Get the event host
                var thePerson = GetCurrentUser();

                //Get the event
                var theEvent = GetEventById(eventId);

                //Remove from the list
                SessionHelper.Events.RemoveGuest(guestId, eventId);

                //Get list of pending invitation ids
                var pendingEventInvitations = SessionHelper.Events.GetGuestList(eventId);
                var personFriendsList = GetPersonFriendList(thePerson);

                //Populate the guset list
                var viewModel = GetEventViewModel(theEvent);
                viewModel.PeopleInvited = GetSelectedGuests(pendingEventInvitations, personFriendsList, eventId);

                response.Data = RenderRazorViewToString("_InvitedPeopleTemplate", viewModel);

                //Save to the database last and only if the event exists in the database
                if (eventId != 0)
                    _eventRepository.SubmitChanges();
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_REMOVE_GUEST_FAIL;
            }

            return Json(response);
        }

        #endregion

        #region Food Items

        /// <summary>
        /// Get a list of all food items belonging to the current user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPersonFoodList(int eventId, int personId, string contains)
        {
            var response = new Response { Error = false };

            try
            {
                //Get list of pending food ids for this event from session
                var pendingEventFoodItemIds = SessionHelper.Events.GetPendingFoodItems(eventId);
                var pendingPersonFoodItemIds = SessionHelper.Person.GetPendingFoodItems(personId);

                var foodList = GetNonSelectedFoodItems(pendingEventFoodItemIds, pendingPersonFoodItemIds, eventId)
                    .Where(x => x.Title.ToLower().Contains(contains.ToLower()));

                response.Data = foodList;
            }
            catch (Exception)
            {
                //TODO: log to database
                response.Error = true;
                response.Message = "An error occurred. Unable to retrieve your list of food items.";
            }

            return Json(response);
        }
        /// <summary>
        /// Get a single food item from the database by the specified id
        /// </summary>
        /// <param name="foodItemId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSingleFoodItem(int foodItemId)
        {
            var response = new Response { Error = false };

            try
            {
                var foodItem = _foodRepository.GetAll().FirstOrDefault(x => x.FoodItemId == foodItemId);
                response.Data = new FoodItemViewModel(foodItem);
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_GET_FOOD_ITEM_FAIL;
            }

            return Json(response);            
        }
        /// <summary>
        /// Add a food item to a user's personal list of food items.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddFoodItem(EventBaseViewModel model)
        {
            var response = new Response { Error = false };

            try
            {
                //Add to the food items table if it doesn't exist
                var newFoodItem = new FoodItem { Title = model.AddFoodItem.Title, Description = model.AddFoodItem.Description };
                _foodRepository.Insert(newFoodItem);
                _foodRepository.SubmitChanges();

                //Add to the user's personal list of food items
                var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == model.PersonId);
                thePerson.MyFoodItems.Add(newFoodItem);

                //Add the pending food item to session
                SessionHelper.Events.AddFoodItem(newFoodItem.FoodItemId, model.EventId);
                SessionHelper.Person.AddFoodItem(newFoodItem.FoodItemId, model.PersonId);

                //Get list of pending food ids for this event from session
                var pendingEventFoodItemIds = SessionHelper.Events.GetPendingFoodItems(model.EventId);
                var pendingPersonFoodItemIds = SessionHelper.Person.GetPendingFoodItems(model.PersonId);

                //Populate the food items already ing brought
                var foodList = GetSelectedFoodItems(pendingEventFoodItemIds, pendingPersonFoodItemIds, model.EventId);
                response.Data = RenderRazorViewToString("_FoodItemListTemplate", foodList);

                //Save to the database if no errors have occurred
                _personRepository.SubmitChanges();
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_ADD_FOOD_ITEM_FAIL;
            }

            return Json(response);
        }
        /// <summary>
        /// Add a game to an event
        /// </summary>
        /// <param name="foodItemId"></param>
        /// <param name="eventId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddExistingFoodItem(int foodItemId, int eventId, int personId)
        {
            var response = new Response { Error = false };

            try
            {
                //Get the food item id
                var foodItem = _foodRepository.GetAll().FirstOrDefault(x => x.FoodItemId == foodItemId);

                //Add the pending food item to session
                SessionHelper.Events.AddFoodItem(foodItem.FoodItemId, eventId);
                SessionHelper.Person.AddFoodItem(foodItem.FoodItemId, personId);

                //Get list of pending food ids for this event from session
                var pendingEventFoodItemIds = SessionHelper.Events.GetPendingFoodItems(eventId);
                var pendingPersonFoodItemIds = SessionHelper.Person.GetPendingFoodItems(personId);

                //Populate the food items already ing brought
                var foodList = GetSelectedFoodItems(pendingEventFoodItemIds, pendingPersonFoodItemIds, eventId);
                response.Data = RenderRazorViewToString("_FoodItemListTemplate", foodList);
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_ADD_FOOD_ITEM_FAIL;
            }

            return Json(response);
        }
        /// <summary>
        /// Update a food item.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateFoodItem(EventBaseViewModel model)
        {
            var response = new Response { Error = false };

            try
            {
                var personId = _userService.GetCurrentUserId(User.Identity.Name);
  
                //Update the food item
                var updateMe = _foodRepository.GetAll().FirstOrDefault(x => x.FoodItemId == model.UpdateFoodItem.FoodItemId);
                updateMe.Title = model.UpdateFoodItem.Title;
                updateMe.Description = model.UpdateFoodItem.Description;

                //Get list of pending food ids for this event from session
                var pendingEventFoodItemIds = SessionHelper.Events.GetPendingFoodItems(model.EventId);
                var pendingPersonFoodItemIds = SessionHelper.Person.GetPendingFoodItems(personId);

                var foodList = GetSelectedFoodItems(pendingEventFoodItemIds, pendingPersonFoodItemIds, model.EventId);
                response.Data = RenderRazorViewToString("_FoodItemListTemplate", foodList);

                //Save to the database last
                _foodRepository.SubmitChanges();
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_UPDATE_FOOD_ITEM_FAIL;
            }

            return Json(response);
        }
        /// <summary>
        /// Remove a food item to the current event or to the user's personal list of food items.
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="foodItemId">The id of the food item to be removed</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveFoodItem(int eventId, int foodItemId)
        {
            var response = new Response { Error = false };

            try
            {
                var personId = _userService.GetCurrentUserId(User.Identity.Name);
                var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == personId);

                //Remove from the list
                SessionHelper.Events.RemoveFoodItem(foodItemId, eventId);

                //Get list of pending food ids for this event from session
                var pendingEventFoodItemIds = SessionHelper.Events.GetPendingFoodItems(eventId);
                var pendingPersonFoodItemIds = SessionHelper.Person.GetPendingFoodItems(personId);

                var foodList = GetSelectedFoodItems(pendingEventFoodItemIds, pendingPersonFoodItemIds, eventId);
                response.Data = RenderRazorViewToString("_FoodItemListTemplate", foodList);

                //Save to the database last and only if the event exists in the database
                if(eventId != 0)
                    _eventRepository.SubmitChanges();
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_REMOVE_FOOD_ITEM_FAIL;
            }

            return Json(response);
        }

        #endregion

        #region Games

        /// <summary>
        /// Get a list of all games belonging to the current user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPersonGameList(int eventId, int personId, string contains)
        {
            var response = new Response { Error = false };

            try
            {
                //Get list of pending game ids for this event from session
                var pendingEventGameIds = SessionHelper.Events.GetPendingGames(eventId);
                var pendingPersonGameIds = SessionHelper.Person.GetPendingGames(personId);

                response.Data = GetNonSelectedGames(pendingEventGameIds, pendingPersonGameIds, eventId)
                    .Where(x => x.Title.ToLower().Contains(contains.ToLower()));
            }
            catch (Exception)
            {
                //TODO: log to database
                response.Error = true;
                response.Message = "An error occurred. Unable to retrieve your list of games.";
            }

            return Json(response);
        }
        /// <summary>
        /// Get a single game from the database
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSingleGame(int gameId)
        {
            var response = new Response { Error = false };

            try
            {
                var game = _gameRepository.GetAll().FirstOrDefault(x => x.GameId == gameId);
                response.Data = new GameViewModel(game);
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_GET_GAME_FAIL;
            }

            return Json(response);
        }
        /// <summary>
        /// Add a game to a user's personal list of games.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddGame(EventBaseViewModel model)
        {
            var response = new Response { Error = false };

            try
            {
                //Add to the games table if it doesn't exist
                var newGame = new Game { Title = model.AddGameItem.Title, Description = model.AddGameItem.Description};
                _gameRepository.Insert(newGame);
                _gameRepository.SubmitChanges();

                //Add to the user's personal list of food items
                var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == model.PersonId);
                thePerson.MyGames.Add(newGame);

                //Add the pending game to session
                SessionHelper.Events.AddGame(newGame.GameId, model.EventId);
                SessionHelper.Person.AddGame(newGame.GameId, model.PersonId);

                //Get list of pending game ids for this event from session
                var pendingEventGameIds = SessionHelper.Events.GetPendingGames(model.EventId);
                var pendingPersonGameIds = SessionHelper.Person.GetPendingGames(model.PersonId);

                //Populate the games already ing brought
                var gameList = GetSelectedGames(pendingEventGameIds, pendingPersonGameIds, model.EventId);
                response.Data = RenderRazorViewToString("_GameListTemplate", gameList);

                //Save to the database last
                _personRepository.SubmitChanges();
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_ADD_FOOD_ITEM_FAIL;
            }

            return Json(response);
        }
        /// <summary>
        /// Add a game to an event
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="eventId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddExistingGame(int gameId, int eventId, int personId)
        {
            var response = new Response { Error = false };

            try
            {
                //Add the pending game to session
                SessionHelper.Events.AddGame(gameId, eventId);
                SessionHelper.Person.AddGame(gameId, personId);

                //Get list of pending game ids for this event from session
                var pendingEventGameIds = SessionHelper.Events.GetPendingGames(eventId);
                var pendingPersonGameIds = SessionHelper.Person.GetPendingGames(personId);

                //Populate the games already ing brought
                var gameList = GetSelectedGames(pendingEventGameIds, pendingPersonGameIds, eventId);
                response.Data = RenderRazorViewToString("_GameListTemplate", gameList);
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_ADD_FOOD_ITEM_FAIL;
            }

            return Json(response);
        }
        /// <summary>
        /// Update properties of the specified game in the view model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateGame(EventBaseViewModel model)
        {
            var response = new Response { Error = false };

            try
            {
                var personId = _userService.GetCurrentUserId(User.Identity.Name);

                //Update the food item
                var updateMe = _gameRepository.GetAll().FirstOrDefault(x => x.GameId == model.UpdateGameItem.GameId);
                updateMe.Title = model.UpdateGameItem.Title;
                updateMe.Description = model.UpdateGameItem.Description;

                //Save to the database last
                _gameRepository.SubmitChanges();

                //Get list of pending food ids for this event from session
                var pendingEventFoodItemIds = SessionHelper.Events.GetPendingGames(model.EventId);
                var pendingPersonFoodItemIds = SessionHelper.Person.GetPendingGames(personId);

                var selectedGames = GetSelectedGames(pendingEventFoodItemIds, pendingPersonFoodItemIds, model.EventId);
                response.Data = RenderRazorViewToString("_GameListTemplate", selectedGames);
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_UPDATE_GAME_FAIL;
            }

            return Json(response);
        }
        /// <summary>
        /// Remove a game to the current event or from the user's personal list of games.
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="gameId">The game id being removed from the event</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveGame(int eventId, int gameId)
        {
            var response = new Response { Error = false };

            try
            {
                //Get the person id
                var personId = _userService.GetCurrentUserId(User.Identity.Name);

                //Remove from the event
                SessionHelper.Events.RemoveGame(gameId, eventId);

                //Get list of pending game ids for this event from session
                var pendingEventGameIds = SessionHelper.Events.GetPendingGames(eventId);
                var pendingPersonGameIds = SessionHelper.Person.GetPendingGames(personId);

                var gameList = GetSelectedGames(pendingEventGameIds, pendingPersonGameIds, eventId);
                response.Data = RenderRazorViewToString("_GameListTemplate", gameList);

                //Save to the database last
                if (eventId != 0)
                    _eventRepository.SubmitChanges();
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_REMOVE_FOOD_ITEM_FAIL;
            }

            return Json(response);
        }

        #endregion

        #region Other

        /// <summary>
        /// Get a list of the times that are used to autocomplete an event's start and end time.
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult GetTimeList()
        {
            var response = new Response { Error = false, Message = ""};

            try
            {
               response.Data = JsonConvert.SerializeObject(_eventService.GetTimeList());
            }
            catch (Exception)
            {
                //TODO: log to database
                response.Error = true;
                response.Message = "An error occured while trying to get the event times list";
            }

            return Json(response);
        }
        /// <summary>
        /// This is a dummy method that is only used as a place holder for AJAX forms. It does nothing.
        /// </summary>
        /// <returns></returns>
        public ActionResult Dummy()
        {
            return Json("");
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Get the HTML out put of the specified view name and model combination.
        /// </summary>
        /// <param name="viewName">The specified partial view</param>
        /// <param name="model">The specified model name</param>
        /// <remarks>Got the code for this @ http://stackoverflow.com/questions/483091/render-a-view-as-a-string</remarks>
        /// <returns></returns>
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        private List<FoodItemViewModel> GetSelectedFoodItems(List<int> eventFoodItemIds, List<int> personFoodItemIds, int eventId)
        {
            var foodList = new List<FoodItemViewModel>();
            var selectedFoodItemIds = personFoodItemIds.Intersect(eventFoodItemIds);
            var remainingFoodItems = _foodRepository.GetAll()
                .Where(x => selectedFoodItemIds.Contains(x.FoodItemId))
                .OrderBy(x => x.Title).ToList();
            remainingFoodItems.ForEach(x =>
                {
                    var viewModel = new FoodItemViewModel(x);
                    viewModel.Index = remainingFoodItems.IndexOf(x);
                    viewModel.EventId = eventId;
                    foodList.Add(viewModel);
                });

            return foodList;
        }

        private List<FoodItemViewModel> GetNonSelectedFoodItems(List<int> eventFoodItemIds, List<int> personFoodItemIds, int eventId)
        {
            var foodList = new List<FoodItemViewModel>();
            var selectedFoodItemIds = personFoodItemIds.Where(x => !eventFoodItemIds.Contains(x));
            var remainingFoodItems = _foodRepository.GetAll()
                .Where(x => selectedFoodItemIds.Contains(x.FoodItemId))
                .OrderBy(x => x.Title).ToList();
            remainingFoodItems.ForEach(x =>
            {
                var viewModel = new FoodItemViewModel(x);
                viewModel.Index = remainingFoodItems.IndexOf(x);
                viewModel.EventId = eventId;
                foodList.Add(viewModel);
            });

            return foodList;
        }

        private List<GameViewModel> GetSelectedGames(List<int> eventGameIds, List<int> personGameIds, int eventId)
        {
            var gameList = new List<GameViewModel>();
            var selectedGamesIds = personGameIds.Intersect(eventGameIds);
            var remainingGames = _gameRepository.GetAll()
                .Where(x => selectedGamesIds.Contains(x.GameId))
                .OrderBy(x => x.Title).ToList();
            remainingGames.ForEach(x =>
                {
                    var viewModel = new GameViewModel(x);
                    viewModel.Index = remainingGames.IndexOf(x);
                    viewModel.EventId = eventId;
                    gameList.Add(viewModel);
            });

            return gameList;
        }

        private List<GameViewModel> GetNonSelectedGames(List<int> eventGameIds, List<int> personGameIds, int eventId)
        {
            var gameList = new List<GameViewModel>();
            var selectedGamesIds = personGameIds.Where(x => !eventGameIds.Contains(x));
            var remainingGames = _gameRepository.GetAll()
                .Where(x => selectedGamesIds.Contains(x.GameId))
                .OrderBy(x => x.Title).ToList();
            remainingGames.ForEach(x =>
            {
                var viewModel = new GameViewModel(x);
                viewModel.Index = remainingGames.IndexOf(x);
                viewModel.EventId = eventId;
                gameList.Add(viewModel);
            });

            return gameList;
        }

        private List<PersonViewModel> GetSelectedGuests(List<int> eventGuestIds, List<int> personFriendIds, int eventId)
        {
            var guestList = new List<PersonViewModel>();
            var selectedGuestIds = personFriendIds.Intersect(eventGuestIds);
            _personRepository.GetAll()
                .Where(x => selectedGuestIds.Contains(x.PersonId))
                .OrderBy(x => x.FirstName)
                .ToList().ForEach(x =>
                {
                    var viewModel = new PersonViewModel(x);
                    guestList.Add(viewModel);
                });

            //Make the unregistered numbers posative
            var unRegisteredIds = new List<int>();
            selectedGuestIds.Where(x => x < 0)
                .ToList()
                .ForEach(x => unRegisteredIds.Add(Math.Abs(x)));

            _inviteRepository.GetAll()
                .Where(x => unRegisteredIds.Contains(x.PendingInvitationId))
                .OrderBy(x => x.FirstName)
                .ToList().ForEach(x =>
                {
                    var viewModel = new PersonViewModel
                        {
                            PersonId = -x.PendingInvitationId,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Email = x.Email,
                            IsRegistered = false
                        };
                    guestList.Add(viewModel);
                });

            return guestList;
        }

        private List<PersonViewModel> GetNonSelectedGuests(List<int> eventGuestIds, List<int> personFriendIds, int eventId)
        {
            var guestList = new List<PersonViewModel>();
            var unSelectedGuestIds = personFriendIds.Where(x => !eventGuestIds.Contains(x));
            _personRepository.GetAll()
                .Where(x => unSelectedGuestIds.Contains(x.PersonId))
                .OrderBy(x => x.FirstName)
                .ToList().ForEach(x =>
                {
                    var viewModel = new PersonViewModel(x);
                    guestList.Add(viewModel);
                });

            //Make the unregistered numbers posative
            var unRegisteredIds = new List<int>();
            unSelectedGuestIds.Where(x => x < 0)
                .ToList()
                .ForEach(x => unRegisteredIds.Add(Math.Abs(x)));

            _inviteRepository.GetAll()
                .Where(x => unRegisteredIds.Contains(x.PendingInvitationId))
                .OrderBy(x => x.FirstName)
                .ToList().ForEach(x =>
                {
                    var viewModel = new PersonViewModel
                    {
                        PersonId = -x.PendingInvitationId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email,
                        IsRegistered = false
                    };
                    guestList.Add(viewModel);
                });

            return guestList;
        }

        private List<int> GetPersonFriendList(Person thePerson)
        {
            var personFriendsList = new List<int>();

            thePerson.MyRegisteredFriends.ForEach(x => personFriendsList.Add(x.PersonId));
            //Make the pending ids negative to avoid conflicts with normal ids
            thePerson.MyUnRegisteredFriends.ForEach(x => personFriendsList.Add(-x.PendingInvitationId));

            return personFriendsList;
        }
        /// <summary>
        /// Get a new instance of the edit event view model with the accepted and declined attendee lists populated
        /// </summary>
        /// <param name="theEvent">An event</param>
        /// <returns></returns>
        private EditEventViewModel GetEventViewModel(Event theEvent)
        {
            var viewModel = new EditEventViewModel();
            viewModel.EventId = theEvent.EventId;
            theEvent.PeopleWhoAccepted.ForEach(x => viewModel.PeopleWhoAccepted.Add(new PersonViewModel(x)));
            theEvent.PeopleWhoDeclined.ForEach(x => viewModel.PeopleWhoDeclined.Add(new PersonViewModel(x)));

            return viewModel;
        }
        /// <summary>
        /// Get the person who is currently logged in
        /// </summary>
        /// <returns></returns>
        private Person GetCurrentUser()
        {
            var userId = _userService.GetCurrentUserId(User.Identity.Name);
            return _personRepository.GetAll().FirstOrDefault(x => x.PersonId == userId);
        }
        /// <summary>
        /// Get a person by the specified person id
        /// </summary>
        /// <param name="personId">A person id</param>
        /// <returns></returns>
        private Person GetPersonById(int personId)
        {
            return _personRepository.GetAll().FirstOrDefault(x => x.PersonId == personId);
        }
        /// <summary>
        /// Get an event by the specified event id
        /// </summary>
        /// <param name="eventId">An event id</param>
        /// <returns></returns>
        private Event GetEventById(int eventId)
        {
            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);

            if (theEvent == null)
            {
                theEvent = new Event
                    {
                        RegisteredInvites = new List<Person>(),
                        UnRegisteredInvites = new List<PendingInvitation>(),
                        PeopleWhoAccepted = new List<Person>(),
                        PeopleWhoDeclined = new List<Person>()
                    };
            }

            return theEvent;
        }

        #endregion
    }
}
