using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Data;
using Web.Data.Models;
using Web.Extensions;

namespace Web.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<Event> _eventRepository;

        public NotificationService(IRepository<Person> personRepo, IRepository<Event> eventRepo)
        {
            _personRepository = personRepo;
            _eventRepository = eventRepo;
        }

        public void SendEmailNotification(List<SystemNotification> notifications)
        {
            //TODO: add logic to actually send an email
            throw new NotImplementedException();
        }

        public void SendFacebookMessage(List<SystemNotification> notifications)
        {
            //TODO: Add logic that will actually send a facebook message.
            throw new NotImplementedException();
        }

        public List<SystemNotification> GetNotificationsForEventUpdate(int eventId)
        {
            var notifications = new List<SystemNotification>();
            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
            _personRepository.GetAll().Where(x => x.MyEvents.Select(y => y.EventId == eventId).Any())
                .ToList().ForEach(x => notifications.Add(new SystemNotification
                    {
                        PersonId = x.UserId,
                        IsFacebookNotification = x.NotifyWithFacebook,
                        Message = string.Format(Constants.MESSAGE_UPDATE_TEMPLATE, 
                            theEvent.Title, theEvent.Coordinator.FirstName, 
                            theEvent.Coordinator.LastName, 
                            theEvent.StartDate.ToShortDateString(),
                            theEvent.StartDate.ToShortTimeString()),
                        Title = Constants.MESSAGE_UPDATE_TITLE
                    }));

            return notifications;
        }

        public List<SystemNotification> GetNotificationsForEventCancelled(int eventId)
        {
            var notifications = new List<SystemNotification>();
            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
            _personRepository.GetAll().Where(x => x.MyEvents.Select(y => y.EventId == eventId).Any())
                .ToList().ForEach(x => notifications.Add(new SystemNotification
                {
                    PersonId = x.UserId,
                    IsFacebookNotification = x.NotifyWithFacebook,
                    Message = string.Format(Constants.MESSAGE_CANCELLED_TEMPLATE,
                        theEvent.Title, theEvent.Coordinator.FirstName,
                        theEvent.Coordinator.LastName),
                    Title = Constants.MESSAGE_CANCELLED_TITLE
                }));

            return notifications;
        }

        public SystemNotification NotifyInvitationAccepted(int eventId, int acceptingId)
        {
            var notification = new SystemNotification();

            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
            var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.UserId == acceptingId);

            notification.IsFacebookNotification = thePerson.NotifyWithFacebook;
            notification.PersonId = acceptingId;
            notification.Title = Constants.MESSAGE_ACCEPT_TITLE;
            notification.Message = string.Format(Constants.MESSAGE_ACCEPT_TEMPLATE, thePerson.FirstName,
                                                 thePerson.LastName,
                                                 theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                 theEvent.StartDate.ToShortTimeString());

            return notification;
        }

        public SystemNotification NotifyInvitationDeclined(int eventId, int decliningId)
        {
            var notification = new SystemNotification();

            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
            var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.UserId == decliningId);

            notification.IsFacebookNotification = thePerson.NotifyWithFacebook;
            notification.PersonId = decliningId;
            notification.Title = Constants.MESSAGE_DECLINE_TITLE;
            notification.Message = string.Format(Constants.MESSAGE_DECLINE_TEMPLATE, thePerson.FirstName,
                                                 thePerson.LastName,
                                                 theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                 theEvent.StartDate.ToShortTimeString());

            return notification;
        }

        public SystemNotification NotifyPersonRemovedFromEvent(int eventId, int removeThisPersonId)
        {
            var notification = new SystemNotification();

            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
            var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.UserId == removeThisPersonId);

            notification.IsFacebookNotification = thePerson.NotifyWithFacebook;
            notification.PersonId = removeThisPersonId;
            notification.Title = Constants.MESSAGE_REMOVE_TITLE;
            notification.Message = string.Format(Constants.MESSAGE_REMOVE_TEMPLATE, thePerson.FirstName,
                                                 thePerson.LastName,
                                                 theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                 theEvent.StartDate.ToShortTimeString());

            return notification;
        }
    }
}