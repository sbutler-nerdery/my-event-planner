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
            _userService = userService;
            _eventService = eventService;
        }

        #endregion

        #region Methods
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
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
                var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == personId);

                var foodList = GetNonSelectedFoodItems(theEvent, thePerson)
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
        /// Add a food item to a user's personal list of food items.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddFoodItem(EventBaseViewModel model)
        {
            var response = new Response { Error = false };

            try
            {
                //Get the event 
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == model.EventId);

                //Add to the food items table if it doesn't exist
                var newFoodItem = new FoodItem { Title = model.AddFoodItem.Title, Description = model.AddFoodItem.Description };
                _foodRepository.Insert(newFoodItem);
                _foodRepository.SubmitChanges();

                //Add to the user's collection of food items
                var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == model.PersonId);
                thePerson.MyFoodItems.Add(newFoodItem);
                theEvent.FoodItems.Add(newFoodItem);

                //Populate the food items already ing brought
                var foodList = GetSelectedFoodItems(theEvent, thePerson);
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
                //Get the event 
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);

                //Get the person
                var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == personId);

                //Get the food item
                var foodItem = _foodRepository.GetAll().FirstOrDefault(x => x.FoodItemId == foodItemId);

                //Add to the event (but don't save it yet... this is just for building the correct view model)
                theEvent.FoodItems.Add(foodItem);

                //Populate the food items already ing brought
                var foodList = GetSelectedFoodItems(theEvent, thePerson).OrderBy(x => x.Title);
                response.Data = RenderRazorViewToString("_FoodItemListTemplate", foodList);

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
                //Remove from the event
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
                var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == personId);
                var removeMe = _foodRepository.GetAll().FirstOrDefault(x => x.FoodItemId == foodItemId);

                theEvent.FoodItems.Remove(removeMe);

                var foodList = GetSelectedFoodItems(theEvent, thePerson);
                response.Data = RenderRazorViewToString("_FoodItemListTemplate", foodList);

                //Save to the database if no errors have occured
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
        /// <summary>
        /// Get a list of all games belonging to the current user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPersonGameList(int eventId, int personId)
        {
            var response = new Response { Error = false };

            try
            {
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
                var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == personId);
                response.Data = GetNonSelectedGames(theEvent, thePerson);
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
        /// Add a game to a user's personal list of games.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddGame(EventBaseViewModel model)
        {
            var response = new Response { Error = false };

            try
            {
                //Get the event 
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == model.EventId);

                //Add to the food items table if it doesn't exist
                var newGame = new Game { Title = model.AddGameItem.Title, Description = model.AddGameItem.Description};
                _gameRepository.Insert(newGame);
                _gameRepository.SubmitChanges();

                //Add to the user's collection of games
                var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == model.PersonId);
                thePerson.MyGames.Add(newGame);
                theEvent.Games.Add(newGame);

                //Populate the games already ing brought
                var gameList = GetSelectedGames(theEvent, thePerson);
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
                //Get the event 
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);

                //Get the person
                var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == personId);

                //Get the game
                var game = _gameRepository.GetAll().FirstOrDefault(x => x.GameId == gameId);

                //Add to the event (but don't save it yet... this is just for building the correct view model)
                theEvent.Games.Add(game);

                //Populate the games already ing brought
                var gameList = GetSelectedGames(theEvent, thePerson);
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
                var personId = _userService.GetCurrentUserId(User.Identity.Name);
                //Remove from the event
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
                var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == personId);
                var removeMe = _gameRepository.GetAll().FirstOrDefault(x => x.GameId == gameId);

                theEvent.Games.Remove(removeMe);

                var gameList = GetSelectedGames(theEvent, thePerson);
                response.Data = RenderRazorViewToString("_GameListTemplate", gameList);

                //Save to the database last
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
        /// <summary>
        /// Get a list of the times that are used to autocomplete an event's start and end time.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Find out if the user's email address already exists in the system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FindUserByEmail(EditEventViewModel model)
        {
            var response = new Response { Error = false };

            try
            {
                var exists = _personRepository.GetAll().FirstOrDefault(x => x.Email == model.EmailInvite.Email);

                var userName = model.EmailInvite.FirstName + " " + model.EmailInvite.LastName;
                response.Data = new { PersonId = 0, model.EmailInvite.Email, UserName = userName, model.EmailInvite.FirstName, model.EmailInvite.LastName, model.EmailInvite.InviteControlId };

                if(exists != null){
                    response.Data = new { exists.PersonId, exists.Email, exists.UserName, exists.FirstName, exists.LastName, model.EmailInvite.InviteControlId };
                }
            }
            catch (Exception ex)
            {
                //TODO: write to database
                response.Error = true;
                response.Message = "Error while trying to retrieve user account by email.";
            }

            return Json(response);
        }

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

        private List<FoodItemViewModel> GetSelectedFoodItems(Event theEvent, Person thePerson)
        {
            var foodList = new List<FoodItemViewModel>();
            var eventFoodItemIds = theEvent.FoodItems.Select(x => x.FoodItemId);
            var myFoodItemIds = thePerson.MyFoodItems.Select(x => x.FoodItemId);
            var selectedFoodItemIds = myFoodItemIds.Intersect(eventFoodItemIds);
            var remainingFoodItems = theEvent.FoodItems
                .Where(x => selectedFoodItemIds.Contains(x.FoodItemId))
                .OrderBy(x => x.Title).ToList();
            remainingFoodItems.ForEach(x =>
            {
                var viewModel = new FoodItemViewModel(x);
                viewModel.Index = remainingFoodItems.IndexOf(x);
                viewModel.EventId = theEvent.EventId;
                foodList.Add(viewModel);
            });

            return foodList;
        }

        private List<FoodItemViewModel> GetNonSelectedFoodItems(Event theEvent, Person thePerson)
        {
            var foodList = new List<FoodItemViewModel>();
            var eventFoodItemIds = theEvent.FoodItems.Select(x => x.FoodItemId);
            var myFoodItemIds = thePerson.MyFoodItems.Select(x => x.FoodItemId);
            var selectedFoodItemIds = myFoodItemIds.Where(x => !eventFoodItemIds.Contains(x));
            var remainingFoodItems = thePerson.MyFoodItems
                .Where(x => selectedFoodItemIds.Contains(x.FoodItemId))
                .OrderBy(x => x.Title).ToList();
            remainingFoodItems.ForEach(x =>
            {
                var viewModel = new FoodItemViewModel(x);
                viewModel.Index = remainingFoodItems.IndexOf(x);
                viewModel.EventId = theEvent.EventId;
                foodList.Add(viewModel);
            });

            return foodList;
        }

        private List<GameViewModel> GetSelectedGames(Event theEvent, Person thePerson)
        {
            var gameList = new List<GameViewModel>();
            var eventGameIds = theEvent.Games.Select(x => x.GameId);
            var myGameIds = thePerson.MyGames.Select(x => x.GameId);
            var selectedGamesIds = myGameIds.Intersect(eventGameIds);
            var remainingGames = theEvent.Games
                .Where(x => selectedGamesIds.Contains(x.GameId))
                .OrderBy(x => x.Title).ToList();
            remainingGames.ForEach(x =>
            {
                var viewModel = new GameViewModel(x);
                viewModel.Index = remainingGames.IndexOf(x);
                viewModel.EventId = theEvent.EventId;
                gameList.Add(viewModel);
            });

            return gameList;
        }

        private List<GameViewModel> GetNonSelectedGames(Event theEvent, Person thePerson)
        {
            var gameList = new List<GameViewModel>();
            var eventGameIds = theEvent.Games.Select(x => x.GameId);
            var myGameIds = thePerson.MyGames.Select(x => x.GameId);
            var selectedGamesIds = myGameIds.Where(x => !eventGameIds.Contains(x));
            var remainingGames = thePerson.MyGames
                .Where(x => selectedGamesIds.Contains(x.GameId))
                .OrderBy(x => x.Title).ToList();
            remainingGames.ForEach(x =>
            {
                var viewModel = new GameViewModel(x);
                viewModel.Index = remainingGames.IndexOf(x);
                viewModel.EventId = theEvent.EventId;
                gameList.Add(viewModel);
            });

            return gameList;
        }

        #endregion
    }
}
