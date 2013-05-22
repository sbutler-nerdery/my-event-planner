using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
        private readonly IRepository<PendingInvitation> _invitationRepository;
        private readonly SmtpClient _emailClient;

        public NotificationService(IRepository<Person> personRepo, IRepository<Event> eventRepo, IRepository<PendingInvitation> invitationRepo)
        {
            _personRepository = personRepo;
            _eventRepository = eventRepo;
            _invitationRepository = invitationRepo;
            _emailClient = new SmtpClient();
        }

        public void SendNotifications(List<EventPlannerNotification> notifications)
        {
            //TODO: Add logic that will actually send a facebook message.
            foreach (var eventPlannerNotification in notifications)
            {
                if (eventPlannerNotification.SendToEmail)
                {
                    var email = new MailMessage();
                    email.To.Add(eventPlannerNotification.Email);
                    email.IsBodyHtml = true;
                    email.Subject = eventPlannerNotification.Title;
                    email.Body = eventPlannerNotification.Message;
                    _emailClient.Send(email);
                }
            }
        }

        public EventPlannerNotification GetNotificationForEventUpdate(int eventId, int registeredId, int nonRegisteredId)
        {
            var theEvent = _eventRepository.GetAll().IncludeAll("Coordinator").FirstOrDefault(x => x.EventId == eventId);
            var registeredPerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == registeredId);
            var nonRegisteredPerson =
                _invitationRepository.GetAll().FirstOrDefault(x => x.PendingInvitationId == nonRegisteredId);

            var notification = new EventPlannerNotification
                {
                    SendToFacebook = (registeredPerson != null) ? registeredPerson.NotifyWithFacebook : (nonRegisteredPerson.FacebookId != null),
                    SendToEmail = (registeredPerson != null) ? registeredPerson.NotifyWithEmail : (nonRegisteredPerson.Email != null),
                    Email = (registeredPerson != null) ? registeredPerson.Email : nonRegisteredPerson.Email,
                    FacebookId = (registeredPerson != null) ? registeredPerson.FacebookId : nonRegisteredPerson.FacebookId,
                    Message = string.Format(Constants.MESSAGE_UPDATE_TEMPLATE,
                                            theEvent.Title, theEvent.Coordinator.FirstName,
                                            theEvent.Coordinator.LastName,
                                            theEvent.StartDate.ToShortDateString(),
                                            theEvent.StartDate.ToShortTimeString()),
                    Title = Constants.MESSAGE_UPDATE_TITLE
                };

            return notification;
        }

        public EventPlannerNotification GetNotificationForEventCancelled(int eventId, int registeredId, int nonRegisteredId)
        {
            var theEvent = _eventRepository.GetAll().IncludeAll("Coordinator").FirstOrDefault(x => x.EventId == eventId);
            var registeredPerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == registeredId);
            var nonRegisteredPerson =
                _invitationRepository.GetAll().FirstOrDefault(x => x.PendingInvitationId == nonRegisteredId);

            var notification = new EventPlannerNotification
            {
                SendToFacebook = (registeredPerson != null) ? registeredPerson.NotifyWithFacebook : (nonRegisteredPerson.FacebookId != null),
                SendToEmail = (registeredPerson != null) ? registeredPerson.NotifyWithEmail : (nonRegisteredPerson.Email != null),
                Email = (registeredPerson != null) ? registeredPerson.Email : nonRegisteredPerson.Email,
                FacebookId = (registeredPerson != null) ? registeredPerson.FacebookId : nonRegisteredPerson.FacebookId,
                Message = string.Format(Constants.MESSAGE_CANCELLED_TEMPLATE,
                        theEvent.Title, theEvent.Coordinator.FirstName,
                        theEvent.Coordinator.LastName),
                Title = Constants.MESSAGE_CANCELLED_TITLE
            };

            return notification;
        }

        public EventPlannerNotification GetNewInvitationNotification(int eventId, int registeredId, int nonRegisteredId, string invitationUrl)
        {
            var notification = new EventPlannerNotification();

            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
            var registeredPerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == registeredId);
            var nonRegisteredPerson = _invitationRepository.GetAll().FirstOrDefault(x => x.PendingInvitationId == nonRegisteredId);

            notification.SendToFacebook = (registeredPerson != null) ? registeredPerson.NotifyWithFacebook : (nonRegisteredPerson.FacebookId != null);
            notification.SendToEmail = (registeredPerson != null) ? registeredPerson.NotifyWithEmail : (nonRegisteredPerson.Email != null);
            notification.Email = (registeredPerson != null) ? registeredPerson.Email : nonRegisteredPerson.Email;
            notification.FacebookId = (registeredPerson != null) ? registeredPerson.FacebookId : nonRegisteredPerson.FacebookId;
            notification.Title = Constants.MESSAGE_NEW_TITLE;
            notification.Message = string.Format(Constants.MESSAGE_NEW_TEMPLATE, theEvent.Coordinator.FirstName,
                                                 theEvent.Coordinator.LastName,
                                                 theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                 theEvent.StartDate.ToShortTimeString(),
                                                 invitationUrl);

            return notification;            
        }

        public EventPlannerNotification GetInvitationAcceptedNotification(int eventId, int acceptingId)
        {
            var notification = new EventPlannerNotification();

            var theEvent = _eventRepository.GetAll().IncludeAll("Coordinator").FirstOrDefault(x => x.EventId == eventId);
            var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == acceptingId);

            var foodItemsText = "";
            var gamesText = "";

            notification.SendToFacebook = theEvent.Coordinator.NotifyWithFacebook;
            notification.SendToEmail = theEvent.Coordinator.NotifyWithEmail;
            notification.PersonId = theEvent.Coordinator.PersonId;
            notification.Email = theEvent.Coordinator.Email;
            notification.FacebookId = theEvent.Coordinator.FacebookId;

            //Set up the text stuff
            notification.Title = Constants.MESSAGE_ACCEPT_TITLE;
            notification.Message = string.Format(Constants.MESSAGE_ACCEPT_TEMPLATE, thePerson.FirstName,
                                                 thePerson.LastName,
                                                 theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                 theEvent.StartDate.ToShortTimeString(),
                                                 foodItemsText, gamesText);

            return notification;
        }

        public EventPlannerNotification GetInvitationDeclinedNotification(int eventId, int decliningId)
        {
            var notification = new EventPlannerNotification();

            var theEvent = _eventRepository.GetAll().IncludeAll("Coordinator").FirstOrDefault(x => x.EventId == eventId);
            var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == decliningId);

            //How does the coordinator want to be notified?
            notification.SendToFacebook = theEvent.Coordinator.NotifyWithFacebook;
            notification.SendToEmail = theEvent.Coordinator.NotifyWithEmail;
            notification.PersonId = theEvent.Coordinator.PersonId;
            notification.Email = theEvent.Coordinator.Email;
            notification.FacebookId = theEvent.Coordinator.FacebookId;

            //Set up the text stuff...
            notification.Title = Constants.MESSAGE_DECLINE_TITLE;
            notification.Message = string.Format(Constants.MESSAGE_DECLINE_TEMPLATE, thePerson.FirstName,
                                                 thePerson.LastName,
                                                 theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                 theEvent.StartDate.ToShortTimeString());

            return notification;
        }

        public EventPlannerNotification GetPersonRemovedFromEventNotification(int eventId, int removeThisPersonId)
        {
            var notification = new EventPlannerNotification();

            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
            var thePerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == removeThisPersonId);

            notification.SendToFacebook = thePerson.NotifyWithFacebook;
            notification.SendToEmail = thePerson.NotifyWithEmail;
            notification.Email = thePerson.Email;
            notification.FacebookId = thePerson.FacebookId;
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