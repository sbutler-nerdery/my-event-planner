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
                var people = new List<SelectListItem>();
                var coordinator = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == userId);
                coordinator.MyRegisteredFriends
                                 .ToList()
                                 .ForEach(
                                     x => people.Add(new SelectListItem
                                     {
                                         Value = x.PersonId.ToString(),
                                         Text = x.FirstName + " " + x.LastName
                                     }));

                coordinator.MyNonRegisteredFriends
                                 .ToList()
                                 .ForEach(
                                     x => people.Add(new SelectListItem
                                     {
                                         Value = (x.Email != null) ? x.Email + "|" + x.FirstName + "|" + x.LastName : x.FacebookId + "|" + x.FirstName + " " + x.LastName,
                                         Text = x.FirstName + " " + x.LastName
                                     }));

                model.PeopleList = new MultiSelectList(people, "Value", "Text");
                model.TimeList = _eventService.GetTimeList();
                model.FacebookFriends = _userService.GetFacebookFriends(userName);

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

                    //Send notifications
                    SendUpdateNotifications(createMe, createMe.RegisteredInvites, createMe.NonRegisteredInvites);

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
                var model = GetViewModel(dataModel);

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
            var updateMe = _eventRepository.GetAll().IncludeAll("Coordinator").FirstOrDefault(x => x.EventId == model.EventId);

            try
            {
                if (ModelState.IsValid)
                {
                    var initialRequiredFieldsState = _eventService.GetSerializedModelState(updateMe);
                    var previousRegisteredInvites = new List<Person>(updateMe.RegisteredInvites);
                    var previousNonRegisteredInvites = new List<PendingInvitation>(updateMe.NonRegisteredInvites);

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

                    var updatedRequiredFieldsState = _eventService.GetSerializedModelState(updateMe);
                    var newRegisteredInvites = _eventService.GetRegisteredInvites(previousRegisteredInvites, updateMe.RegisteredInvites);
                    var newNonRegisteredInvites = _eventService.GetNonRegisteredInvites(previousNonRegisteredInvites, updateMe.NonRegisteredInvites);
                    var uninvitedRegisteredUsers = _eventService.GetRegisteredUninvites(previousRegisteredInvites, newRegisteredInvites);
                    var uninvitedNonRegisteredUsers = _eventService.GetNonRegisteredUninvites(previousNonRegisteredInvites, newNonRegisteredInvites);

                    //Send notifications if the model has changed
                    if (!initialRequiredFieldsState.Equals(updatedRequiredFieldsState))
                        SendUpdateNotifications(updateMe, newRegisteredInvites, newNonRegisteredInvites);
                    //Even if the model state has not changed, send notifications to newly invited people
                    else
                    {
                        SendInvitations(updateMe, newRegisteredInvites, newNonRegisteredInvites);
                    }

                    //No matter what let people know when they are uninvited
                    SendUnInvitations(updateMe, uninvitedRegisteredUsers, uninvitedNonRegisteredUsers);

                    return RedirectToAction("Index", "Home", new {message = BaseControllerMessageId.SaveModelSuccess});
                }

                model = GetViewModel(updateMe);

                //Return the model if the state is invalid...
                return View(model);
            }
            catch (Exception)
            {
                //TODO: log to database
            }

            //If it makes it this far something is wrong.
            model = GetViewModel(updateMe);
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

        #region Helpers

        private EventViewModel GetViewModel(Event dataModel)
        {
            var model = new EventViewModel(dataModel);

            //Populate the total list of people who could be invited to an event.
            var userName = (User != null) ? User.Identity.Name : string.Empty;
            var userId = _userService.GetCurrentUserId(userName);
            var people = new List<SelectListItem>();
            var coordinator = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == userId);
            coordinator.MyRegisteredFriends
                             .ToList()
                             .ForEach(
                                 x => people.Add(new SelectListItem
                                 {
                                     Value = x.PersonId.ToString(),
                                     Text = x.FirstName + " " + x.LastName
                                 }));

            coordinator.MyNonRegisteredFriends
                             .ToList()
                             .ForEach(
                                 x => people.Add(new SelectListItem
                                 {
                                     Value = (x.Email != null) ? x.Email + "|" + x.FirstName + "|" + x.LastName : x.FacebookId + "|" + x.FirstName + " " + x.LastName,
                                     Text = x.FirstName + " " + x.LastName
                                 }));

            model.PeopleList = new MultiSelectList(people, "Value", "Text");
            model.TimeList = _eventService.GetTimeList();
            model.FacebookFriends = _userService.GetFacebookFriends(userName);

            return model;
        }

        /// <summary>
        /// Send update notifications to everyone who was invited.
        /// </summary>
        /// <param name="theEvent"></param>
        /// <param name="newRegisteredInvites"></param>
        /// <param name="newNonRegisteredInvites"></param>
        private void SendUpdateNotifications(Event theEvent, List<Person> newRegisteredInvites, List<PendingInvitation> newNonRegisteredInvites)
        {
            //This is here so that unit tests on this controller will pass. So much easier than faking it!
            if (Request == null)
                return;

            var notifications = new List<EventPlannerNotification>();

            //Only send update notification to people who have already been invited
            theEvent.RegisteredInvites
                .Where(x => !newRegisteredInvites.Select(y => y.PersonId).Contains(x.PersonId))
                .ToList().ForEach(x =>
            {
                var updateNotification = _notificationService.GetNotificationForEventUpdate(theEvent.EventId, x.PersonId, 0);
                notifications.Add(updateNotification);
            });

            theEvent.NonRegisteredInvites
                .Where(x => !newNonRegisteredInvites.Select(y => y.PendingInvitationId).Contains(x.PendingInvitationId))
                .ToList().ForEach(x =>
            {
                var updateNotification = _notificationService.GetNotificationForEventUpdate(theEvent.EventId, 0, x.PendingInvitationId);
                notifications.Add(updateNotification);
            });

            _notificationService.SendNotifications(notifications);

            //Invite new people to the event...
            SendInvitations(theEvent, newRegisteredInvites, newNonRegisteredInvites);
        }

        /// <summary>
        /// Send invitations to newly invited people
        /// </summary>
        /// <param name="theEvent">The specified event</param>
        /// <param name="registeredInvites">A list of newly invited people who are registered users</param>
        /// <param name="nonRegisteredInvites">A list of newly invited people who are non-registered users</param>
        private void SendInvitations(Event theEvent, List<Person> registeredInvites, List<PendingInvitation> nonRegisteredInvites)
        {
            //This is here so that unit tests on this controller will pass.
            if (Request == null)
                return;

            var notifications = new List<EventPlannerNotification>();

            registeredInvites.ForEach(x =>
            {
                var notificationUrl = string.Format("{0}://{1}{2}",
                    Request.Url.Scheme,
                    Request.Url.Authority,
                    Url.Action("AcceptInvitation", "Home", new { eventId = theEvent.EventId, accepteeId = x.PersonId }));
                var notification = _notificationService
                    .GetNewInvitationNotification(theEvent.EventId, x.PersonId, 0, notificationUrl);

                notifications.Add(notification);
            });

            nonRegisteredInvites.ForEach(x =>
            {
                var notificationUrl = string.Format("{0}://{1}{2}",
                    Request.Url.Scheme,
                    Request.Url.Authority,
                    Url.Action("Register", "Account", new { eventId = theEvent.EventId, pendingInvitationId = x.PendingInvitationId }));

                var notification = _notificationService
                    .GetNewInvitationNotification(theEvent.EventId, 0, x.PendingInvitationId, notificationUrl);

                notifications.Add(notification);
            });

            _notificationService.SendNotifications(notifications);
        }

        /// <summary>
        /// Send un-invitations 
        /// </summary>
        /// <param name="theEvent">The specified event</param>
        /// <param name="registeredInvites">A list of newly un-invited people who are registered users</param>
        /// <param name="nonRegisteredInvites">A list of newly un-invited people who are non-registered users</param>
        private void SendUnInvitations(Event theEvent, List<Person> registeredInvites, List<PendingInvitation> nonRegisteredInvites)
        {
            //This is here so that unit tests on this controller will pass.
            if (Request == null)
                return;

            var notifications = new List<EventPlannerNotification>();

            registeredInvites.ForEach(x =>
            {
                var notification = _notificationService
                    .GetPersonRemovedFromEventNotification(theEvent.EventId, x.PersonId, 0);

                notifications.Add(notification);
            });

            nonRegisteredInvites.ForEach(x =>
            {
                var notification = _notificationService
                    .GetPersonRemovedFromEventNotification(theEvent.EventId, 0, x.PendingInvitationId);

                notifications.Add(notification);
            });

            _notificationService.SendNotifications(notifications);
        }

        #endregion

    }
}
