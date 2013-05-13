using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Controllers;
using Web.Data;
using Web.Data.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private IRepository<Person> _personRepo;
        private IRepository<Event> _eventRepo;

        [TestInitialize]
        public void Spinup()
        {
            _personRepo = A.Fake<IRepository<Person>>();
            _eventRepo = A.Fake<IRepository<Event>>();
        }
        /// <summary>
        /// Make sure that the status message is correct if an exception is caught
        /// </summary>
        [TestMethod]
        public void Build_View_Model_Fail()
        {
            //Arrange
            var service = A.Fake<IUserService>();
            A.CallTo(() => _personRepo.GetAll()).Throws(new Exception("I can't find the database!"));
            var controller = new HomeController(_personRepo, service);

            //Act
            var result = controller.Index(null) as ViewResult;

            //Assert
            Assert.AreEqual(Constants.BASE_BUILD_VIEW_FAIL, result.ViewBag.StatusMessage);
        }
        /// <summary>
        /// Make sure we get back the view model that we are expecting if the user is found.
        /// </summary>
        [TestMethod]
        public void Build_View_Model_Succeed()
        {
            //Arrange
            var eventOne = new Event {EventId = 1, Title = "Test event 1"};
            var eventTwo = new Event {EventId = 2, Title = "Test event 2"};
            var hostingEvent = new Event { EventId = 3, Title = "My own event"};
            var personResults = new List<Person>
                {
                    new Person{PersonId = 1, 
                        FirstName = "Joe", 
                        LastName = "Smith",
                        MyEvents = new List<Event> {hostingEvent},
                        MyInvitations = new List<Event> { eventOne, eventTwo },
                        AmAttending = new List<Event> { eventOne },
                        HaveDeclined = new List<Event> { eventTwo }
                    }
                };
            var service = A.Fake<IUserService>();
            A.CallTo(() => _personRepo.GetAll()).Returns(personResults.AsQueryable());
            A.CallTo(() => service.GetCurrentUserId("")).Returns(1);
            var controller = new HomeController(_personRepo, service);

            //Act
            var result = controller.Index(null) as ViewResult;
            var model = result.Model as HomeViewModel;

            //Assert
            var acceptedInvitationsCount = model.MyInvitations.Count(x => x.HasAccepted == true && x.HasDeclined == false);
            var declinedInvitationsCount = model.MyInvitations.Count(x => x.HasAccepted == false && x.HasDeclined == true);
            Assert.AreEqual(string.Empty, result.ViewBag.StatusMessage);
            Assert.AreEqual(model.MyEvents.Count, 1);
            Assert.AreEqual(model.MyInvitations.Count, 2);
            Assert.AreEqual(model.HaveDeclined.Count, 1);
            Assert.AreEqual(model.AmAttending.Count, 1);
            Assert.AreEqual(acceptedInvitationsCount, 1);
            Assert.AreEqual(declinedInvitationsCount, 1);
        }
    }
}
