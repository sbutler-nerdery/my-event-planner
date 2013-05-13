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

        public void SendNotifications(List<SystemNotification> notifications)
        {
            //TODO: Add logic that will actually send a facebook message.
            throw new NotImplementedException();
        }

        public List<SystemNotification> GetNotificationsForEventUpdate(int eventId)
        {
            var notifications = new List<SystemNotification>();
            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
            _personRepository.GetAll().Where(x => x.AmAttending.Select(y => y.EventId == eventId).Any() ||
                x.MyInvitations.Select(y => y.EventId == eventId).Any())
                .ToList().ForEach(x => notifications.Add(new SystemNotification
                    {
                        PersonId = x.PersonId,
                        SendToFacebook = x.NotifyWithFacebook,
                        SendToEmail = x.NotifyWithEmail,
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
            _personRepository.GetAll().Where(x => x.AmAttending.Select(y => y.EventId == eventId).Any() ||
                x.MyInvitations.Select(y => y.EventId == eventId).Any())
                .ToList().ForEach(x => notifications.Add(new SystemNotification
                {
                    PersonId = x.PersonId,
                    SendToFacebook = x.NotifyWithFacebook,
                    SendToEmail = x.NotifyWithEmail,
                    Message = string.Format(Constants.MESSAGE_CANCELLED_TEMPLATE,
                        theEvent.Title, theEvent.Coordinator.FirstName,
                        theEvent.Coordinator.LastName),
                    Title = Constants.MESSAGE_CANCELLED_TITLE
                }));

            return notifications;
        }

        public SystemNotification GetNewInvitationNotification(int eventId, int inviteeId, string invitationUrl)
        {
            var notification = new SystemNotification();

            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
            var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == inviteeId);

            notification.SendToFacebook = thePerson.NotifyWithFacebook;
            notification.SendToEmail = thePerson.NotifyWithEmail;
            notification.PersonId = inviteeId;
            notification.Title = Constants.MESSAGE_NEW_TITLE;
            notification.Message = string.Format(Constants.MESSAGE_NEW_TEMPLATE, theEvent.Coordinator.FirstName,
                                                 theEvent.Coordinator.LastName,
                                                 theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                 theEvent.StartDate.ToShortTimeString(),
                                                 invitationUrl);

            return notification;            
        }

        public SystemNotification GetInvitationAcceptedNotification(int eventId, int acceptingId)
        {
            var notification = new SystemNotification();

            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
            var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == acceptingId);

            notification.SendToFacebook = thePerson.NotifyWithFacebook;
            notification.SendToEmail = thePerson.NotifyWithEmail;
            notification.PersonId = acceptingId;
            notification.Title = Constants.MESSAGE_ACCEPT_TITLE;
            notification.Message = string.Format(Constants.MESSAGE_ACCEPT_TEMPLATE, thePerson.FirstName,
                                                 thePerson.LastName,
                                                 theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                 theEvent.StartDate.ToShortTimeString());

            return notification;
        }

        public SystemNotification GetInvitationDeclinedNotification(int eventId, int decliningId)
        {
            var notification = new SystemNotification();

            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
            var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == decliningId);

            notification.SendToFacebook = thePerson.NotifyWithFacebook;
            notification.SendToEmail = thePerson.NotifyWithEmail;
            notification.PersonId = decliningId;
            notification.Title = Constants.MESSAGE_DECLINE_TITLE;
            notification.Message = string.Format(Constants.MESSAGE_DECLINE_TEMPLATE, thePerson.FirstName,
                                                 thePerson.LastName,
                                                 theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                 theEvent.StartDate.ToShortTimeString());

            return notification;
        }

        public SystemNotification GetPersonRemovedFromEventNotification(int eventId, int removeThisPersonId)
        {
            var notification = new SystemNotification();

            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
            var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == removeThisPersonId);

            notification.SendToFacebook = thePerson.NotifyWithFacebook;
            notification.SendToEmail = thePerson.NotifyWithEmail;
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