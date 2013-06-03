using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using SendGridMail;
using SendGridMail.Transport;
using Web.Data;
using Web.Data.Models;
using Web.Extensions;

namespace Web.Services
{
    public class NotificationService : INotificationService
    {
        #region Fields

        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<PendingInvitation> _invitationRepository;
        private readonly SmtpClient _emailClient;
        private readonly bool _useSendGrid = false;

        #endregion

        #region Constructors

        public NotificationService(IRepositoryFactory factory)
        {
            _personRepository = factory.GetRepository<Person>();
            _eventRepository = factory.GetRepository<Event>();
            _invitationRepository = factory.GetRepository<PendingInvitation>();
            _emailClient = new SmtpClient();
            _useSendGrid = bool.Parse(ConfigurationManager.AppSettings["MailWithSendGrid"]);
        }

        #endregion

        #region Methods

        public void SendNotifications(List<EventPlannerNotification> notifications)
        {
            foreach (var eventPlannerNotification in notifications)
            {
                if (eventPlannerNotification.SendToEmail)
                {
                    if (!_useSendGrid)
                    {
                        var email = new MailMessage();
                        email.To.Add(eventPlannerNotification.Email);
                        email.IsBodyHtml = true;
                        email.Subject = eventPlannerNotification.Title;
                        email.Body = eventPlannerNotification.Message;
                        _emailClient.Send(email);
                    }
                    else
                    {
                        //Use Azure sendgrid for email stuff
                        // Setup the email properties.
                        var from = new MailAddress("noreply@eventplanner.net");
                        var to = new[] {new MailAddress(eventPlannerNotification.Email)};
                        var cc = new MailAddress[0];
                        var bcc = new MailAddress[0];
                        var subject = eventPlannerNotification.Title;
                        var html = eventPlannerNotification.Message;
                        var text = eventPlannerNotification.Message;

                        // Create an email, passing in the the eight properties as arguments.
                        var emailMessage = SendGrid.GetInstance(from, to, cc, bcc, subject, html, text);

                        // Create network credentials to access your SendGrid account.
                        var username = "azure_ee7157f9810b7c7e83ffcc1febc5a652@azure.com";
                        var pswd = "jqj9ikpj";

                        var credentials = new NetworkCredential(username, pswd);

                        // Create an SMTP transport for sending email.
                        var transportSMTP = SMTP.GetInstance(credentials);

                        // Send the email.
                        transportSMTP.Deliver(emailMessage);
                    }
                }
            }
        }

        public EventPlannerNotification GetNotificationForEventUpdate(int eventId, int registeredId, int nonRegisteredId)
        {
            var theEvent = _eventRepository.GetAll().IncludeAll("Coordinator").FirstOrDefault(x => x.EventId == eventId);
            var registeredPerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == registeredId);
            var nonRegisteredPerson =
                _invitationRepository.GetAll().FirstOrDefault(x => x.PendingInvitationId == nonRegisteredId);

            var coordinatorName = GetPersonName(theEvent.Coordinator);
            var notification = new EventPlannerNotification
                {
                    SendToFacebook = (registeredPerson != null) ? registeredPerson.NotifyWithFacebook : (nonRegisteredPerson.FacebookId != null),
                    SendToEmail = (registeredPerson != null) ? registeredPerson.NotifyWithEmail : (nonRegisteredPerson.Email != null),
                    Email = (registeredPerson != null) ? registeredPerson.Email : nonRegisteredPerson.Email,
                    FacebookId = (registeredPerson != null) ? registeredPerson.FacebookId : nonRegisteredPerson.FacebookId,
                    Message = string.Format(Constants.MESSAGE_UPDATE_TEMPLATE,
                                            theEvent.Title, coordinatorName,
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

            var coordinatorName = GetPersonName(theEvent.Coordinator);
            var notification = new EventPlannerNotification
            {
                SendToFacebook = (registeredPerson != null) ? registeredPerson.NotifyWithFacebook : (nonRegisteredPerson.FacebookId != null),
                SendToEmail = (registeredPerson != null) ? registeredPerson.NotifyWithEmail : (nonRegisteredPerson.Email != null),
                Email = (registeredPerson != null) ? registeredPerson.Email : nonRegisteredPerson.Email,
                FacebookId = (registeredPerson != null) ? registeredPerson.FacebookId : nonRegisteredPerson.FacebookId,
                Message = string.Format(Constants.MESSAGE_CANCELLED_TEMPLATE,
                        theEvent.Title, coordinatorName),
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
            var coordinatorName = GetPersonName(theEvent.Coordinator);

            notification.SendToFacebook = (registeredPerson != null) ? registeredPerson.NotifyWithFacebook : (nonRegisteredPerson.FacebookId != null);
            notification.SendToEmail = (registeredPerson != null) ? registeredPerson.NotifyWithEmail : (nonRegisteredPerson.Email != null);
            notification.Email = (registeredPerson != null) ? registeredPerson.Email : nonRegisteredPerson.Email;
            notification.FacebookId = (registeredPerson != null) ? registeredPerson.FacebookId : nonRegisteredPerson.FacebookId;
            notification.Title = Constants.MESSAGE_NEW_TITLE;
            notification.Message = string.Format(Constants.MESSAGE_NEW_TEMPLATE, coordinatorName,
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
            var personName = GetPersonName(thePerson);

            var foodItemsText = "";
            var gamesText = "";

            notification.SendToFacebook = theEvent.Coordinator.NotifyWithFacebook;
            notification.SendToEmail = theEvent.Coordinator.NotifyWithEmail;
            notification.PersonId = theEvent.Coordinator.PersonId;
            notification.Email = theEvent.Coordinator.Email;
            notification.FacebookId = theEvent.Coordinator.FacebookId;

            //Set up the text stuff
            notification.Title = Constants.MESSAGE_ACCEPT_TITLE;
            notification.Message = string.Format(Constants.MESSAGE_ACCEPT_TEMPLATE, personName,
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
            var personName = GetPersonName(thePerson);

            notification.Title = Constants.MESSAGE_DECLINE_TITLE;
            notification.Message = string.Format(Constants.MESSAGE_DECLINE_TEMPLATE, personName,
                                                 theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                 theEvent.StartDate.ToShortTimeString());

            return notification;
        }

        public EventPlannerNotification GetPersonRemovedFromEventNotification(int eventId, int registeredId, int nonRegisteredId)
        {
            var notification = new EventPlannerNotification();

            var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
            var registeredPerson = _personRepository.GetAll().FirstOrDefault(x => x.PersonId == registeredId);
            var nonRegisteredPerson = _invitationRepository.GetAll().FirstOrDefault(x => x.PendingInvitationId == nonRegisteredId);
            var coordinatorName = GetPersonName(theEvent.Coordinator);

            notification.SendToFacebook = (registeredPerson != null) ? registeredPerson.NotifyWithFacebook : (nonRegisteredPerson.FacebookId != null);
            notification.SendToEmail = (registeredPerson != null) ? registeredPerson.NotifyWithEmail : (nonRegisteredPerson.Email != null);
            notification.Email = (registeredPerson != null) ? registeredPerson.Email : nonRegisteredPerson.Email;
            notification.FacebookId = (registeredPerson != null) ? registeredPerson.FacebookId : nonRegisteredPerson.FacebookId;
            notification.Title = Constants.MESSAGE_REMOVE_TITLE;
            notification.Message = string.Format(Constants.MESSAGE_REMOVE_TEMPLATE, coordinatorName,
                                                 theEvent.Title, 
                                                 theEvent.StartDate.ToShortDateString(),
                                                 theEvent.StartDate.ToShortTimeString());

            return notification;
        }

        private string GetPersonName(Person thePerson)
        {
            var personName = (string.IsNullOrEmpty(thePerson.FirstName) || string.IsNullOrEmpty(thePerson.LastName))
                                 ? thePerson.UserName
                                 : string.Format("{0} {1}", thePerson.FirstName, thePerson.LastName);
            return personName;
        }

        #endregion
    }
}