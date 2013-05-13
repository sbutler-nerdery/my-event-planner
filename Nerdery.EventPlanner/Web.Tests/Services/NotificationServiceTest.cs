using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Data;
using Web.Data.Models;
using Web.Services;

namespace Web.Tests.Services
{
    [TestClass]
    public class NotificationServiceTest
    {
        private NotificationService _service;
        private IRepository<Person> _personRepo;
        private IRepository<Event> _eventRepo;

        [TestInitialize]
        public void Spinup()
        {
            _personRepo = A.Fake<IRepository<Person>>();
            _eventRepo = A.Fake<IRepository<Event>>();
        }

        /// <summary>
        /// This ensures that the correct notification is created when an event coordinator 
        /// invites a new person to join an event.
        /// </summary>
        [TestMethod]
        public void Notification_On_New_Invitation()
        {
            //Arrange
            var inivitationUrl = "http://mysite.com/accept=1";
            var theHost = new Person { PersonId = 1, FirstName = "Joe", LastName = "Smith" };
            var thePerson = new Person { PersonId = 2, FirstName = "Sally", LastName = "Hart", NotifyWithFacebook = true, NotifyWithEmail = false };
            var theEvent = new Event { EventId = 1, Title = "My Test Event", StartDate = DateTime.Now, Coordinator = theHost};
            var personList = new List<Person> { thePerson, theHost };
            var eventList = new List<Event> { theEvent };

            var service = new NotificationService(_personRepo, _eventRepo);
            A.CallTo(() => _personRepo.GetAll()).Returns(personList.AsQueryable());
            A.CallTo(() => _eventRepo.GetAll()).Returns(eventList.AsQueryable());

            //Act
            var notification = service.GetNewInvitationNotification(theEvent.EventId, thePerson.PersonId, inivitationUrl);

            //Assert            
            string expectedMessage = string.Format(Constants.MESSAGE_NEW_TEMPLATE, theHost.FirstName,
                                                    theHost.LastName,
                                                    theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                    theEvent.StartDate.ToShortTimeString(),
                                                    inivitationUrl);

            Assert.AreEqual(notification.PersonId, thePerson.PersonId);
            Assert.AreEqual(notification.SendToEmail, personList[0].NotifyWithEmail);
            Assert.AreEqual(notification.SendToFacebook, personList[0].NotifyWithFacebook);
            Assert.AreEqual(notification.Title, Constants.MESSAGE_NEW_TITLE);
            Assert.AreEqual(notification.Message, expectedMessage);
        }
        /// <summary>
        /// This test ensures that if an attendee is removed from an event they are notified.
        /// </summary>
        [TestMethod]
        public void Notification_On_Attendee_Removed()
        {
            //Arrange
            var thePerson = new Person{PersonId = 1, FirstName = "Joe", LastName = "Smith", NotifyWithFacebook = true, NotifyWithEmail = false};
            var theEvent = new Event{EventId = 1, Title = "My Test Event", StartDate = DateTime.Now };
            var personList = new List<Person>{thePerson};
            var eventList = new List<Event>{theEvent};

            var service = new NotificationService(_personRepo, _eventRepo);
            A.CallTo(() => _personRepo.GetAll()).Returns(personList.AsQueryable());
            A.CallTo(() => _eventRepo.GetAll()).Returns(eventList.AsQueryable());

            //Act
            var notification = service.GetPersonRemovedFromEventNotification(theEvent.EventId, thePerson.PersonId);

            //Assert            
            string expectedMessage = string.Format(Constants.MESSAGE_REMOVE_TEMPLATE, thePerson.FirstName,
                                                    thePerson.LastName,
                                                    theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                    theEvent.StartDate.ToShortTimeString());

            Assert.AreEqual(notification.PersonId, thePerson.PersonId);
            Assert.AreEqual(notification.SendToEmail, personList[0].NotifyWithEmail);
            Assert.AreEqual(notification.SendToFacebook, personList[0].NotifyWithFacebook);
            Assert.AreEqual(notification.Title, Constants.MESSAGE_REMOVE_TITLE);
            Assert.AreEqual(notification.Message, expectedMessage);
        }
        /// <summary>
        /// This test ensures that if an event's start date is updated all of the users who are
        /// invited or have accepted receive a notification
        /// </summary>
        [TestMethod]
        public void Notification_On_Event_Dates_Updated()
        {
            //Arrange
            var theHost = new Person {PersonId = 3, FirstName = "Matt", LastName = "Harmin"};
            var theEvent = new Event { EventId = 1, Title = "My Test Event", StartDate = DateTime.Now,
                Coordinator = theHost };
            var peopleList = new List<Person>
                {
                    new Person {PersonId = 1, FirstName = "Joe", LastName = "Smith", AmAttending = new List<Event>{theEvent}, MyInvitations = new List<Event>()},
                    new Person {PersonId = 2, FirstName = "Sally", LastName = "Hardy", AmAttending = new List<Event>(), MyInvitations = new List<Event>{theEvent}},
                };

            A.CallTo(() => _eventRepo.GetAll()).Returns(new List<Event> { theEvent }.AsQueryable());
            A.CallTo(() => _personRepo.GetAll()).Returns(peopleList.AsQueryable());

            var service = new NotificationService(_personRepo, _eventRepo);

            //Act
            var notifications = service.GetNotificationsForEventUpdate(theEvent.EventId);
            var firstNotification = notifications[0];

            //Assert
            string expectedMessage = string.Format(Constants.MESSAGE_UPDATE_TEMPLATE, theEvent.Title,
                                                    theHost.FirstName,
                                                    theHost.LastName,
                                                    theEvent.StartDate.ToShortDateString(),
                                                    theEvent.StartDate.ToShortTimeString());

            Assert.AreEqual(notifications.Count, 2);
            Assert.AreEqual(firstNotification.Title, Constants.MESSAGE_UPDATE_TITLE);
            Assert.AreEqual(firstNotification.Message, expectedMessage);
        }
        /// <summary>
        /// This ensures that the correct notification is created for the event coordinator
        /// if an invitee accepts an event invitation.
        /// </summary>
        [TestMethod]
        public void Notification_On_Invitation_Accepted()
        {
            //Arrange
            var thePerson = new Person { PersonId = 1, FirstName = "Joe", LastName = "Smith", NotifyWithFacebook = true, NotifyWithEmail = false};
            var theEvent = new Event { EventId = 1, Title = "My Test Event", StartDate = DateTime.Now };
            var personList = new List<Person> { thePerson };
            var eventList = new List<Event> { theEvent };

            var service = new NotificationService(_personRepo, _eventRepo);
            A.CallTo(() => _personRepo.GetAll()).Returns(personList.AsQueryable());
            A.CallTo(() => _eventRepo.GetAll()).Returns(eventList.AsQueryable());

            //Act
            var notification = service.GetInvitationAcceptedNotification(theEvent.EventId, thePerson.PersonId);

            //Assert            
            string expectedMessage = string.Format(Constants.MESSAGE_ACCEPT_TEMPLATE, thePerson.FirstName,
                                                    thePerson.LastName,
                                                    theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                    theEvent.StartDate.ToShortTimeString());

            Assert.AreEqual(notification.PersonId, thePerson.PersonId);
            Assert.AreEqual(notification.SendToEmail, personList[0].NotifyWithEmail);
            Assert.AreEqual(notification.SendToFacebook, personList[0].NotifyWithFacebook);
            Assert.AreEqual(notification.Title, Constants.MESSAGE_ACCEPT_TITLE);
            Assert.AreEqual(notification.Message, expectedMessage);
        }
        /// <summary>
        /// This ensures that the correct notification is created for the event coordinator
        /// if an invitee declines an event invitation.
        /// </summary>
        [TestMethod]
        public void Notification_On_Invitation_Declined()
        {
            //Arrange
            var thePerson = new Person { PersonId = 1, FirstName = "Joe", LastName = "Smith", NotifyWithFacebook = true, NotifyWithEmail = true};
            var theEvent = new Event { EventId = 1, Title = "My Test Event", StartDate = DateTime.Now };
            var personList = new List<Person> { thePerson };
            var eventList = new List<Event> { theEvent };

            var service = new NotificationService(_personRepo, _eventRepo);
            A.CallTo(() => _personRepo.GetAll()).Returns(personList.AsQueryable());
            A.CallTo(() => _eventRepo.GetAll()).Returns(eventList.AsQueryable());

            //Act
            var notification = service.GetInvitationDeclinedNotification(theEvent.EventId, thePerson.PersonId);

            //Assert            
            string expectedMessage = string.Format(Constants.MESSAGE_DECLINE_TEMPLATE, thePerson.FirstName,
                                                    thePerson.LastName,
                                                    theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                    theEvent.StartDate.ToShortTimeString());

            Assert.AreEqual(notification.PersonId, thePerson.PersonId);
            Assert.AreEqual(notification.SendToEmail, personList[0].NotifyWithEmail);
            Assert.AreEqual(notification.SendToFacebook, personList[0].NotifyWithFacebook);
            Assert.AreEqual(notification.Title, Constants.MESSAGE_DECLINE_TITLE);
            Assert.AreEqual(notification.Message, expectedMessage);
        }
        /// <summary>
        /// This test ensures that if an event is deleted from the system, the appropriate notifications are 
        /// generated.
        /// </summary>
        [TestMethod]
        public void Notification_On_Event_Cancelled()
        {
            //Arrange
            var theHost = new Person { PersonId = 3, FirstName = "Matt", LastName = "Harmin" };
            var theEvent = new Event
            {
                EventId = 1,
                Title = "My Test Event",
                StartDate = DateTime.Now,
                Coordinator = theHost
            };
            var peopleList = new List<Person>
                {
                    new Person {PersonId = 1, FirstName = "Joe", LastName = "Smith", AmAttending = new List<Event>{theEvent}, MyInvitations = new List<Event>()},
                    new Person {PersonId = 2, FirstName = "Sally", LastName = "Hardy", AmAttending = new List<Event>(), MyInvitations = new List<Event>{theEvent}},
                };

            A.CallTo(() => _eventRepo.GetAll()).Returns(new List<Event> { theEvent }.AsQueryable());
            A.CallTo(() => _personRepo.GetAll()).Returns(peopleList.AsQueryable());

            var service = new NotificationService(_personRepo, _eventRepo);

            //Act
            var notifications = service.GetNotificationsForEventCancelled(theEvent.EventId);
            var firstNotification = notifications[0];

            //Assert
            string expectedMessage = string.Format(Constants.MESSAGE_CANCELLED_TEMPLATE, theEvent.Title,
                                                    theHost.FirstName,
                                                    theHost.LastName);

            Assert.AreEqual(notifications.Count, 2);
            Assert.AreEqual(firstNotification.Title, Constants.MESSAGE_CANCELLED_TITLE);
            Assert.AreEqual(firstNotification.Message, expectedMessage);
        }
    }
}
