using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Web.Data;
using Web.Data.Models;
using Web.Extensions;
using Web.Services;
using WebMatrix.WebData;

namespace Web.Controllers
{
    /// <summary>
    /// This controller is used exclusively for AJAX calls. All of the methods
    /// return JSON objects.
    /// </summary>
    public class ServiceController : BaseController
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<FoodItem> _foodRepository;
        private readonly IRepository<Game> _gameRepository;
        private readonly IUserService _userService;
        private readonly IEventService _eventService;

        public ServiceController(IRepository<Event> eventRepo, IRepository<Person> personRepo, IRepository<FoodItem> foodRepo, 
            IRepository<Game> gameRepo, IUserService userService, IEventService eventService)
        {
            _eventRepository = eventRepo;
            _personRepository = personRepo;
            _foodRepository = foodRepo;
            _gameRepository = gameRepo;
            _userService = userService;
            _eventService = eventService;
        }
        /// <summary>
        /// Add a food item to a user's personal list of food items.
        /// </summary>
        /// <param name="personId">The specified perosn id</param>
        /// <param name="title">The title for the food item</param>
        /// <param name="description">The description for the food item</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddFoodItem(int personId, string title, string description)
        {
            var response = new Response { Error = false };

            try
            {
                //Add to the food items table if it doesn't exist
                var newFoodItem = new FoodItem {Title = title, Description = description};
                _foodRepository.Insert(newFoodItem);
                _foodRepository.SubmitChanges();

                //Add to the user's collection of food items
                var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == personId);
                thePerson.MyFoodItems.Add(newFoodItem);
                _personRepository.SubmitChanges();

                response.Data = newFoodItem;
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
        /// <param name="foodItemJson">A serialized food item</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveFoodItem(int eventId, string foodItemJson)
        {
            var response = new Response { Error = false };

            try
            {
                //Remove from the event if the event id > 0
                //Remove from the user's collection of food items if the event id == 0
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
        /// Add a game to a user's personal list of games.
        /// </summary>
        /// <param name="title">The title for the game</param>
        /// <param name="description">The description for the game</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddGame(int personId, string title, string description)
        {
            var response = new Response { Error = false };

            try
            {
                //Add to the food items table if it doesn't exist
                var newGame = new Game { Title = title, Description = description };
                _gameRepository.Insert(newGame);
                _gameRepository.SubmitChanges();

                //Add to the user's collection of food items
                var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == personId);
                thePerson.MyGames.Add(newGame);
                _personRepository.SubmitChanges();

                response.Data = newGame;
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
        /// <param name="gameJson">A serialized game</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveGame(int eventId, string gameJson)
        {
            var response = new Response { Error = false };

            try
            {
                //Remove from the event if the event id > 0
                //Remove from the user's collection of games if the event id == 0
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
    }
}
