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
                        IsFacebookNotification = x.NotifyWithFacebook,
                        Message = string.Format("The event title '{0}' hosted by {1} {2} has will now start on {3} at {4}.", 
                            theEvent.Title, theEvent.Coordinator.FirstName, 
                            theEvent.Coordinator.LastName, 
                            theEvent.StartDate.ToShortDateString(),
                            theEvent.StartDate.ToShortTimeString()),
                        Title = "Event Update"
                    }));

            return notifications;
        }

        public List<SystemNotification> GetNotificationsForEventCancelled(int eventId)
        {
            throw new NotImplementedException();
        }

        public SystemNotification NotifyInvitationAccepted(int eventId, int acceptingId)
        {
            throw new NotImplementedException();
        }

        public SystemNotification NotifyInvitationDeclined(int eventId, int decliningId)
        {
            throw new NotImplementedException();
        }
    }
}