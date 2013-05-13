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
    public class EventController : Controller
    {
        #region Fields

        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IEventService _eventService;
        private readonly IUserService _userService;

        #endregion

        #region Constructors

        public EventController(IRepository<Event> eventRepo, IRepository<Person> personRepo, 
            IEventService service, IUserService userService)
        {
            _eventRepository = eventRepo;
            _personRepository = personRepo;
            _eventService = service;
            _userService = userService;
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// Create a new event
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var model = new EventViewModel();

            //Populate the total list of people who could be invited to an event.
            _personRepository.GetAll()
                             .FirstOrDefault(x => x.PersonId == _userService.GetCurrentUserId(User.Identity.Name))
                             .MyFriends
                             .ToList()
                             .ForEach(
                                 x =>
                                 model.PeopleList.Add(new PersonViewModel
                                     {
                                         PersonId = x.PersonId,
                                         FirstName = x.FirstName,
                                         LastName = x.LastName
                                     }));

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var createMe = model.GetDataModel();

                //Update the date / time for the event
                createMe.StartDate = _eventService.GetEventStartDate(model.StartDate, model.StartTime);
                createMe.EndDate = _eventService.GetEventEndDate(createMe.StartDate, model.EndTime);

                //Invite people
                _eventService.InviteNewPeople(createMe, model);

                _eventRepository.Insert(createMe);
                _eventRepository.SubmitChanges();
            }
            return View();
        }

        /// <summary>
        /// Edit an existing event
        /// </summary>
        /// <param name="id">The specified event id</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var dataModel = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == id);
            var model = new EventViewModel(dataModel);

            //Populate the total list of people who could be invited to an event.
            _personRepository.GetAll()
                             .FirstOrDefault(x => x.PersonId == _userService.GetCurrentUserId(User.Identity.Name))
                             .MyFriends
                             .ToList()
                             .ForEach(
                                 x =>
                                 model.PeopleList.Add(new PersonViewModel
                                 {
                                     PersonId = x.PersonId,
                                     FirstName = x.FirstName,
                                     LastName = x.LastName
                                 }));
            return View();
        }

        [HttpPost]
        public ActionResult Edit(EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateMe = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == model.EventId);

                updateMe.Title = model.Title;
                updateMe.Description = model.Description;
                updateMe.Location = model.Location;
                updateMe.Coordinator = model.Coordinator.GetDataModel();

                //Update the date / time for the event
                updateMe.StartDate = _eventService.GetEventStartDate(model.StartDate, model.StartTime);
                updateMe.EndDate = _eventService.GetEventEndDate(updateMe.StartDate, model.EndTime);

                //Update the collection properties
                
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
            }
            return View();
        }

        /// <summary>
        /// Delete an event, which is the same as cancelling an event.
        /// </summary>
        /// <param name="id">The specified event id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
