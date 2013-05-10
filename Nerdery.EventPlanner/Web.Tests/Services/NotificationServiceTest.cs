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
        /// This test ensures that if an attendee is removed from an event they are notified.
        /// </summary>
        [TestMethod]
        public void Notification_On_Attendee_Removed()
        {
            //Arrange
            var thePerson = new Person{UserId = 1, FirstName = "Joe", LastName = "Smith", NotifyWithFacebook = true};
            var theEvent = new Event{EventId = 1, Title = "My Test Event", StartDate = DateTime.Now };
            var personList = new List<Person>{thePerson};
            var eventList = new List<Event>{theEvent};

            var service = new NotificationService(_personRepo, _eventRepo);
            A.CallTo(() => _personRepo.GetAll()).Returns(personList.AsQueryable());
            A.CallTo(() => _eventRepo.GetAll()).Returns(eventList.AsQueryable());

            //Act
            var notification = service.NotifyPersonRemovedFromEvent(1, 1);

            //Assert            
            string expectedMessage = string.Format(Constants.MESSAGE_REMOVE_TEMPLATE, thePerson.FirstName,
                                                    thePerson.LastName,
                                                    theEvent.Title, theEvent.StartDate.ToShortDateString(),
                                                    theEvent.StartDate.ToShortTimeString());

            Assert.AreEqual(notification.PersonId, personList[0].UserId);
            Assert.AreEqual(notification.IsFacebookNotification, personList[0].NotifyWithFacebook);
            Assert.AreEqual(notification.Title, Constants.MESSAGE_REMOVE_TITLE);
            Assert.AreEqual(notification.Message, expectedMessage);
        }
        /// <summary>
        /// This test ensures that if an event's start date is updated all of the users receive a notification
        /// </summary>
        [TestMethod]
        public void Notification_On_Event_Dates_Updated()
        {

        }
        /// <summary>
        /// This ensures that the correct notification is created for the event coordinator
        /// if an invitee accepts an event invitation.
        /// </summary>
        [TestMethod]
        public void Notification_On_Invitation_Accepted()
        {

        }
        /// <summary>
        /// This ensures that the correct notification is created for the event coordinator
        /// if an invitee declines an event invitation.
        /// </summary>
        [TestMethod]
        public void Notification_On_Invitation_Declined()
        {

        }
        /// <summary>
        /// This test ensures that if an event is deleted from the system, the appropriate notifications are 
        /// generated.
        /// </summary>
        [TestMethod]
        public void Notification_On_Event_Cancelled()
        {
            //Arrange

            //Act

            //Assert
        }
    }
}
