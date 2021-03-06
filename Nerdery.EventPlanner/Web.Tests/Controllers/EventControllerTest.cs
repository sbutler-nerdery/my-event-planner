﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
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
    public class EventControllerTest : BaseTestFixture
    {
        /// <summary>
        /// This unit test ensures that all of the required fields for the event view model must be filled out on create.
        /// </summary>
        [TestMethod]
        public void Create_Event_Build_ViewModel_Fail()
        {
            //Arrange
            A.CallTo(() => PersonRepo.GetAll()).Returns(null);
            var contoller = new EventController(RepositoryFactory, EventService, UserService, NotifyService);

            //Act
            var result = contoller.Create() as ViewResult;

            //Assert
            Assert.AreEqual(result.ViewBag.StatusMessage, Constants.BASE_BUILD_VIEW_FAIL);
        }

        [TestMethod]
        public void Create_Event_Build_ViewModel_Success()
        {
            //Arrange
            var thePerson = new Person
                {
                    PersonId = 1,
                    UserName = "jsmith",
                    FirstName = "Joe",
                    LastName = "Smith",
                    MyRegisteredFriends = new List<Person>(),
                    MyUnRegisteredFriends = new List<PendingInvitation>(),
                    MyFoodItems = new List<FoodItem>(),
                    MyGames = new List<Game>()
                };
            A.CallTo(() => UserService.GetCurrentUserId("")).Returns(1);
            A.CallTo(() => PersonRepo.GetAll()).Returns(new List<Person> { thePerson }.AsQueryable());
            var contoller = new EventController(RepositoryFactory, EventService, UserService, NotifyService);

            //Act
            var result = contoller.Create() as ViewResult;

            //Assert
            Assert.AreEqual(result.ViewBag.StatusMessage, null);
        }

        /// <summary>
        /// This unit test ensures that the correct message is returned in the query string if creating event fails.
        /// </summary>
        [TestMethod]
        public void Create_Event_Fail()
        {
            //Arrange
            var contoller = new EventController(RepositoryFactory, EventService, UserService, NotifyService);

            //Act
            var result = contoller.Create(new EditEventViewModel()) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "Home");
            Assert.AreEqual(result.RouteValues["message"].ToString(), "SaveModelFailed");
        }

        /// <summary>
        /// This unit test ensures that all of the required fields for the event view model must be filled out on create.
        /// </summary>
        [TestMethod]
        public void Create_Event_ModelState_Not_Valid()
        {
            //Arrange
            var viewModel = new EditEventViewModel();
            var contoller = new EventController(RepositoryFactory, EventService, UserService, NotifyService);

            var modelBinder = new ModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => viewModel, viewModel.GetType()),
                ValueProvider = new NameValueCollectionValueProvider(new NameValueCollection(), CultureInfo.InvariantCulture)
            };

            var binder = new DefaultModelBinder().BindModel(new ControllerContext(), modelBinder);
            contoller.ModelState.Clear();
            contoller.ModelState.Merge(modelBinder.ModelState);
            //Act
            var result = contoller.Create(viewModel) as ViewResult;

            //Assert
            Assert.AreEqual(result.ViewData.ModelState.Count, 6);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        /// <summary>
        /// This unit test ensures that the correct message is returned in the query string if creating event succeeds.
        /// </summary>
        [TestMethod]
        public void Create_Event_Success()
        {
            //Arrange
            var viewModel = GetTestEventViewModel();
            var expectedDataModel = GetTestEventDataModel();
            var contoller = new EventController(RepositoryFactory, EventService, UserService, NotifyService);
            A.CallTo(() => PersonRepo.GetAll()).Returns(new List<Person> {new Person {PersonId = 1}}.AsQueryable());

            //Act
            var result = contoller.Create(viewModel) as RedirectToRouteResult;

            //Assert
            //That the data model being passed to the repository is what we expect.
            A.CallTo(() => EventRepo.Insert(A<Event>._)).MustHaveHappened();
            A.CallTo(() => EventRepo.SubmitChanges()).MustHaveHappened();

            //That the route values are what we expect.
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "Home");
            Assert.AreEqual(result.RouteValues["message"].ToString(), "SaveModelSuccess");
        }

        /// <summary>
        /// This unit test ensures that the correct error message is returned if building the view model fails.
        /// </summary>
        [TestMethod]
        public void Edit_Event_Build_View_Model_Fail()
        {
            //Arrange
            A.CallTo(() => PersonRepo.GetAll()).Returns(null);
            var contoller = new EventController(RepositoryFactory, EventService, UserService, NotifyService);

            //Act
            var result = contoller.Edit(1) as ViewResult;

            //Assert
            Assert.AreEqual(result.ViewBag.StatusMessage, Constants.BASE_BUILD_VIEW_FAIL);
        }

        /// <summary>
        /// This unit test ensures that the correct message is returned in the query string if editing event fails.
        /// </summary>
        [TestMethod]
        public void Edit_Event_Fail()
        {
            //Arrange
            var theHost = new Person
                {
                    PersonId = 0, 
                    FirstName = "Billy", 
                    LastName = "Bob",
                    MyFoodItems = new List<FoodItem>(),
                    MyGames = new List<Game>()
                };
            var theEvent = new Event {
                EventId = 1, 
                Coordinator = theHost,
                Title = "My Event", 
                Description = "It's cool", 
                Location = "My House", 
                StartDate = DateTime.Now,
                RegisteredInvites = new List<Person>(),
                UnRegisteredInvites = new List<PendingInvitation>(),
                FoodItems = new List<FoodItem>(),
                Games = new List<Game>()
            };
            var viewModel = new EditEventViewModel(theEvent);
            var contoller = new EventController(RepositoryFactory, EventService, UserService, NotifyService);

            //Act
            A.CallTo(() => EventRepo.GetAll()).Returns(new List<Event> { theEvent }.AsQueryable());
            A.CallTo(() => PersonRepo.GetAll()).Returns(new List<Person> { theHost }.AsQueryable());
            A.CallTo(() => EventRepo.SubmitChanges()).Throws(new Exception("Crap I crashed."));
            var result = contoller.Edit(viewModel) as ViewResult;

            //Assert
            Assert.AreEqual(result.ViewBag.StatusMessage, Constants.BASE_SAVE_FAIL);
        }

        /// <summary>
        /// Make sure we are getting back the view model that we are expecting
        /// </summary>
        [TestMethod]
        public void Edit_Event_Build_View_Model_Success()
        {
            //Arrange
            var dataModel = GetTestEventDataModel(1);
            var theHost = new Person
            {
                PersonId = 1,
                FirstName = "Billy",
                LastName = "Bob",
                MyFoodItems = new List<FoodItem>(),
                MyGames = new List<Game>()
            };

            var controller = new EventController(RepositoryFactory, EventService, UserService, NotifyService);

            //Act
            A.CallTo(() => EventRepo.GetAll()).Returns(new List<Event> { dataModel }.AsQueryable());
            A.CallTo(() => PersonRepo.GetAll()).Returns(new List<Person> { theHost }.AsQueryable());
            A.CallTo(() => UserService.GetCurrentUserId("")).Returns(1);//the username will be blank in unit tests... easier than mocking a ton of stuff

            var result = controller.Edit(1) as ViewResult;

            //Assert
            Assert.AreEqual(((EditEventViewModel)result.Model).Title, dataModel.Title);
            Assert.AreEqual(result.ViewBag.StatusMessage, string.Empty);
        }

        /// <summary>
        /// This unit test ensures that all of the required fields for the event view model must be filled out on edit.
        /// </summary>
        [TestMethod]
        public void Edit_Event_ModelState_Not_Valid()
        {
            //Arrange
            var theHost = new Person
            {
                PersonId = 1,
                FirstName = "Billy",
                LastName = "Bob",
                MyFoodItems = new List<FoodItem>(),
                MyGames = new List<Game>()
            };
            var theEvent = GetTestEventDataModel(1);
            var viewModel = new EditEventViewModel(theEvent);
            var contoller = new EventController(RepositoryFactory, EventService, UserService, NotifyService);

            var modelBinder = new ModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => viewModel, viewModel.GetType()),
                ValueProvider = new NameValueCollectionValueProvider(new NameValueCollection(), CultureInfo.InvariantCulture)
            };

            A.CallTo(() => EventRepo.GetAll()).Returns(new List<Event> { theEvent }.AsQueryable());
            A.CallTo(() => PersonRepo.GetAll()).Returns(new List<Person> { theHost }.AsQueryable());
            A.CallTo(() => UserService.GetCurrentUserId("")).Returns(1);

            //Act

            //None of these should be null
            viewModel.Title = string.Empty;
            viewModel.Description = string.Empty;
            viewModel.Location = string.Empty;
            viewModel.StartDate = null;
            viewModel.StartTime = string.Empty;
            viewModel.EndTime = string.Empty;

            //Set model binding
            var binder = new DefaultModelBinder().BindModel(new ControllerContext(), modelBinder);
            contoller.ModelState.Clear();
            contoller.ModelState.Merge(modelBinder.ModelState);

            //Get the result
            var result = contoller.Edit(viewModel) as ViewResult;

            //Assert
            Assert.AreEqual(result.ViewData.ModelState.Count, 6);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        /// <summary>
        /// This unit test ensures that the correct message is returned in the query string if editing event succeeds.
        /// </summary>
        [TestMethod]
        public void Edit_Event_Success()
        {
            //Arrange
            var theHost = new Person
            {
                PersonId = 1,
                FirstName = "Billy",
                LastName = "Bob",
                MyFoodItems = new List<FoodItem>(),
                MyGames = new List<Game>()
            };

            var viewModel = GetTestEventViewModel(1);
            var expectedDataModel = GetTestEventDataModel(1);
            expectedDataModel.Coordinator = theHost;

            var contoller = new EventController(RepositoryFactory, EventService, UserService, NotifyService);

            A.CallTo(() => PersonRepo.GetAll()).Returns(new List<Person> { theHost }.AsQueryable());
            A.CallTo(() => EventRepo.GetAll()).Returns(new List<Event> { expectedDataModel }.AsQueryable());

            //Act
            var result = contoller.Edit(viewModel) as RedirectToRouteResult;

            //Assert
            A.CallTo(() => EventRepo.SubmitChanges()).MustHaveHappened();

            //That the route values are what we expect.
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "Home");
            Assert.AreEqual(result.RouteValues["message"].ToString(), "SaveModelSuccess");
        }

        /// <summary>
        /// This unit test ensures that the correct message is returned in the query string if deleting event fails.
        /// </summary>
        [TestMethod]
        public void Delete_Event_Fail()
        {
            //Arrange
            var contoller = new EventController(RepositoryFactory, EventService, UserService, NotifyService);

            //Act
            A.CallTo(() => EventRepo.GetAll()).Throws(new Exception("Database error!"));
            var result = contoller.Delete(1) as RedirectToRouteResult;

            //Assert

            //That the route values are what we expect.
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "Home");
            Assert.AreEqual(result.RouteValues["message"].ToString(), "DeleteFailed");
        }

        /// <summary>
        /// This unit test ensures that the correct message is returned in the query string if deleting event succeeds.
        /// </summary>
        [TestMethod]
        public void Delete_Event_Success()
        {
            //Arrange
            var expectedDataModel = GetTestEventDataModel(1);
            var contoller = new EventController(RepositoryFactory, EventService, UserService, NotifyService);

            //Act
            A.CallTo(() => EventRepo.GetAll()).Returns(new List<Event>{expectedDataModel}.AsQueryable());
            var result = contoller.Delete(1) as RedirectToRouteResult;

            //Assert
            A.CallTo(() => EventRepo.Delete(A<Event>.That.Matches(x => x == expectedDataModel))).MustHaveHappened();
            A.CallTo(() => EventRepo.SubmitChanges()).MustHaveHappened();

            //That the route values are what we expect.
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "Home");
            Assert.AreEqual(result.RouteValues["message"].ToString(), "DeleteSuccessful");
        }
    }
}
