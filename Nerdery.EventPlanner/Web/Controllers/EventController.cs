using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Data;
using Web.Data.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class EventController : BaseController
    {
        #region Fields

        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Constructors

        public EventController(IRepository<Event> eventRepo, IRepository<Person> personRepo, 
            IEventService service, IUserService userService, INotificationService notificationService)
        {
            _eventRepository = eventRepo;
            _personRepository = personRepo;
            _eventService = service;
            _userService = userService;
            _notificationService = notificationService;
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// Create a new event
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            try
            {
                var model = new EventViewModel
                    {
                        StartDate = DateTime.Now.Date,
                        StartTime = "7:00 PM",
                        EndTime = "9:00 PM"
                    };

                //Populate the total list of people who could be invited to an event.
                var userName = (User != null) ? User.Identity.Name : string.Empty;
                var userId = _userService.GetCurrentUserId(userName);
                var people = new List<PersonViewModel>();
                _personRepository.GetAll()
                                 .FirstOrDefault(x => x.PersonId == userId)
                                 .MyFriends
                                 .ToList()
                                 .ForEach(
                                     x => people.Add(new PersonViewModel
                                     {
                                         PersonId = x.PersonId,
                                         UserName = x.FirstName + " " + x.LastName
                                     }));

                model.PeopleList = new MultiSelectList(people, "PersonId", "UserName");
                model.TimeList = _eventService.GetTimeList();
                model.FacebookFriends = new List<PersonViewModel>();

                return View(model);
            }
            catch (Exception)
            {
                //TODO: log in database
                ViewBag.StatusMessage = GetMessageFromMessageId(BaseControllerMessageId.BuildViewModelFail);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(EventViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var createMe = model.GetDataModel();

                    //Set the event coordinator
                    var userName = (User != null) ? User.Identity.Name : string.Empty;
                    var userId = _userService.GetCurrentUserId(userName);
                    var coordinator = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == userId);
                    createMe.Coordinator = coordinator;

                    //Update the date / time for the event
                    _eventService.SetEventDates(createMe, model);

                    //Invite people
                    _eventService.InviteNewPeople(createMe, model);

                    //Add food
                    _eventService.AppendNewFoodItems(createMe, model);

                    //Add games
                    _eventService.AppendNewGames(createMe, model);

                    _eventRepository.Insert(createMe);
                    _eventRepository.SubmitChanges();

                    return RedirectToAction("Index", "Home", new {message = BaseControllerMessageId.SaveModelSuccess});
                }

                //Return the model if the state is invalid...
                return View(model);
            }
            catch (Exception)
            {
                //TODO: log to database
            }

            //If it makes it this far something is wrong.
            return RedirectToAction("Index", "Home", new { message = BaseControllerMessageId.SaveModelFailed });
        }

        /// <summary>
        /// Edit an existing event
        /// </summary>
        /// <param name="id">The specified event id</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                var dataModel = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == id);
                var model = new EventViewModel(dataModel);

                //Populate the total list of people who could be invited to an event.
                var userName = (User != null) ? User.Identity.Name : string.Empty;
                var userId = _userService.GetCurrentUserId(userName);
                var people = new List<PersonViewModel>();
                _personRepository.GetAll()
                                 .FirstOrDefault(x => x.PersonId == userId)
                                 .MyFriends.ToList()
                                 .ForEach(
                                     x => people.Add(new PersonViewModel
                                                 {
                                                     PersonId = x.PersonId,
                                                     UserName = x.FirstName + " " + x.LastName
                                                 }));
                //Get the people who are invited
                model.PeopleList = new MultiSelectList(people, "PersonId", "UserName", model.PeopleInvited);

                //Nothing to report if everything succeeds
                ViewBag.StatusMessage = string.Empty;
                return View(model);
            }
            catch (Exception)
            {
                //TODO: log to database
                ViewBag.StatusMessage = GetMessageFromMessageId(BaseControllerMessageId.BuildViewModelFail);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Edit(EventViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var updateMe = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == model.EventId);

                    updateMe.Title = model.Title;
                    updateMe.Description = model.Description;
                    updateMe.Location = model.Location;

                    //Update the date / time for the event
                    _eventService.SetEventDates(updateMe, model);

                    //Food items
                    _eventService.AppendNewFoodItems(updateMe, model);
                    _eventService.RemoveFoodItems(updateMe, model);

                    //Games
                    _eventService.AppendNewGames(updateMe, model);
                    _eventService.RemoveGames(updateMe, model);

                    //People invited
                    _eventService.InviteNewPeople(updateMe, model);
                    _eventService.UninvitePeople(updateMe, model);

                    _eventRepository.SubmitChanges();

                    return RedirectToAction("Index", "Home", new {message = BaseControllerMessageId.SaveModelSuccess});
                }

                //Return the model if the state is invalid...
                return View(model);
            }
            catch (Exception)
            {
                //TODO: log to database
            }

            //If it makes it this far something is wrong.
            ViewBag.StatusMessage = GetMessageFromMessageId(BaseControllerMessageId.SaveModelFailed);
            return View(model);
        }

        /// <summary>
        /// Delete an event, which is the same as cancelling an event.
        /// </summary>
        /// <param name="id">The specified event id</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            try
            {
                var deleteMe = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == id);
                _eventRepository.Delete(deleteMe);
                _eventRepository.SubmitChanges();

                return RedirectToAction("Index", "Home", new { message = BaseControllerMessageId.DeleteSuccessful });
            }
            catch (Exception)
            {
                //TODO: log to database
            }

            //If it makes it this far something is wrong.
            return RedirectToAction("Index", "Home", new { message = BaseControllerMessageId.DeleteFailed });
        }

        #endregion

    }
}
