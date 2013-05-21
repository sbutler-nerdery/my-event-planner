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
        private readonly IUserService _userService;
        private readonly IEventService _eventService;

        public ServiceController(IRepository<Event> eventRepo, IRepository<Person> personRepo, IUserService userService, IEventService eventService)
        {
            _eventRepository = eventRepo;
            _personRepository = personRepo;
            _userService = userService;
            _eventService = eventService;
        }
        /// <summary>
        /// Add a food item to the current event and / or to the user's personal list of food items.
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="foodItemJson">A serialized food item</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddFoodItem(int eventId, string foodItemJson)
        {
            var response = new Response { Error = false };

            try
            {
                //Add to the food items table if it doesn't exist
                //Add to the event if the event id > 0
                //Add to the user's collection of food items
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
        /// Add a game to the current event and / or to the user's personal list of games.
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="gameJson">A serialized game</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddGame(int eventId, string gameJson)
        {
            var response = new Response { Error = false };

            try
            {
                //Add to the food items table if it doesn't exist
                //Add to the event if the event id > 0
                //Add to the user's collection of games
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
        /// Invite a user to partake in an event by email
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EmailInvite()
        {
            var response = new Response { Error = false, Message = "" };

            try
            {
                //TODO: something...
            }
            catch (Exception)
            {
                //TODO: log to database
                response.Error = true;
                response.Message = "An error occured while trying to send this invitation";
            }

            return Json(response);
        }
        /// <summary>
        /// Invite one or more users to partake in an event by facebook message.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FacebookInvite()
        {
            var response = new Response { Error = false, Message = "" };

            try
            {
                //TODO: something...
            }
            catch (Exception)
            {
                //TODO: log to database
                response.Error = true;
                response.Message = "An error occured while trying to send facebook invitations";
            }

            return Json(response);
        }
    }
}
