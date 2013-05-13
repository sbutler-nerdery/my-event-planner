using System;
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
    public class EventControllerTest
    {
        private IRepository<Person> _personRepo;
        private IRepository<Event> _eventRepo;
        private IUserService _userService;
        private IEventService _eventService;

        [TestInitialize]
        public void SpinUp()
        {
            _personRepo = A.Fake<IRepository<Person>>();
            _eventRepo = A.Fake<IRepository<Event>>();
            _userService = A.Fake<IUserService>();
            _eventService = A.Fake<IEventService>();
        }

        /// <summary>
        /// This unit test ensures that all of the required fields for the event view model must be filled out on create.
        /// </summary>
        [TestMethod]
        public void Create_Event_Fail_Model_Build()
        {
            //Arrange
            A.CallTo(() => _personRepo.GetAll()).Returns(null);
            var contoller = new EventController(_eventRepo, _personRepo, _eventService, _userService);

            //Act
            var result = contoller.Create() as ViewResult;

            //Assert
            Assert.AreEqual(result.ViewBag.StatusMessage, Constants.BASE_BUILD_VIEW_FAIL);
        }

        /// <summary>
        /// This unit test ensures that the correct message is returned in the query string if creating event fails.
        /// </summary>
        [TestMethod]
        public void Create_Event_Fail()
        {
            //Arrange
            A.CallTo(() => _eventRepo.SubmitChanges()).Throws(new Exception("Error saving to the database!"));
            var contoller = new EventController(_eventRepo, _personRepo, _eventService, _userService);

            //Act
            var result = contoller.Create(new EventViewModel()) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "Home");
            Assert.AreEqual(result.RouteValues["message"].ToString(), "SaveModelFailed");
        }

        /// <summary>
        /// This unit test ensures that the correct message is returned in the query string if creating event succeeds.
        /// </summary>
        [TestMethod]
        public void Create_Event_Success()
        {

        }

        /// <summary>
        /// This unit test ensures that all of the required fields for the event view model must be filled out on edit.
        /// </summary>
        [TestMethod]
        public void Edit_Event_Model_Not_Valid()
        {

        }

        /// <summary>
        /// This unit test ensures that the correct message is returned in the query string if editing event fails.
        /// </summary>
        [TestMethod]
        public void Edit_Event_Fail()
        {

        }

        /// <summary>
        /// This unit test ensures that the correct message is returned in the query string if editing event succeeds.
        /// </summary>
        [TestMethod]
        public void Edit_Event_Success()
        {

        }

        /// <summary>
        /// This unit test ensures that the correct message is returned in the query string if deleting event fails.
        /// </summary>
        [TestMethod]
        public void Delete_Event_Fail()
        {

        }

        /// <summary>
        /// This unit test ensures that the correct message is returned in the query string if deleting event succeeds.
        /// </summary>
        [TestMethod]
        public void Delete_Event_Success()
        {

        }

        /// <summary>
        /// This unit test ensures that the correct error message is returned if building the view model fails.
        /// </summary>
        [TestMethod]
        public void Edit_Event_Build_View_Model_Fail()
        {

        }

        /// <summary>
        /// Make sure we are getting back the view model that we are expecting
        /// </summary>
        [TestMethod]
        public void Edit_Event_Build_View_Model_Success()
        {

        }
    }
}
