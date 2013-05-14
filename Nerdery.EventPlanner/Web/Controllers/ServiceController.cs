using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
    public class ServiceController : Controller
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IUserService _userService;

        public ServiceController(IRepository<Event> eventRepo, IRepository<Person> personRepo, IUserService userService)
        {
            _eventRepository = eventRepo;
            _personRepository = personRepo;
            _userService = userService;
        }

        /// <summary>
        /// Decline an invitation to an event
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="userId">The specified user id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeclineInviation(int eventId, int userId)
        {
            var response = new Response { Error = false };

            try
            {
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
                var thePerson =
                    _personRepository.GetAll()
                                     .FirstOrDefault(x => x.PersonId == userId);

                thePerson.AmAttending.Remove(theEvent);
                thePerson.HaveDeclined.Add(theEvent);
                _personRepository.SubmitChanges();
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.BASE_ACCEPT_INVITATION_FAIL;
            }

            return Json(response);
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
    }
}
