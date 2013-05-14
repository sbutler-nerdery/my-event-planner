using System;
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
            _eventService = new EventService();
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
            var contoller = new EventController(_eventRepo, _personRepo, _eventService, _userService);

            //Act
            var result = contoller.Create(new EventViewModel()) as RedirectToRouteResult;

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
            var viewModel = new EventViewModel();
            var contoller = new EventController(_eventRepo, _personRepo, _eventService, _userService);

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
            Assert.AreEqual(result.ViewData.ModelState.Count, 4);
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
            var contoller = new EventController(_eventRepo, _personRepo, _eventService, _userService);

            //Act
            var result = contoller.Create(viewModel) as RedirectToRouteResult;

            //Assert
            //That the data model being passed to the repository is what we expect.
            A.CallTo(() => _eventRepo.Insert(A<Event>.That.Matches(x => x != expectedDataModel))).MustHaveHappened();
            A.CallTo(() => _eventRepo.SubmitChanges()).MustHaveHappened();

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
            A.CallTo(() => _personRepo.GetAll()).Returns(null);
            var contoller = new EventController(_eventRepo, _personRepo, _eventService, _userService);

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
            var contoller = new EventController(_eventRepo, _personRepo, _eventService, _userService);

            //Act
            var result = contoller.Edit(new EventViewModel()) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "Home");
            Assert.AreEqual(result.RouteValues["message"].ToString(), "SaveModelFailed");
        }

        /// <summary>
        /// Make sure we are getting back the view model that we are expecting
        /// </summary>
        [TestMethod]
        public void Edit_Event_Build_View_Model_Success()
        {
            //Arrange
            var dataModel = GetTestEventDataModel();
            var theHost = dataModel.Coordinator;
            var friendOne = new Person{PersonId = 4, FirstName = "Mark", LastName = "Walburg"};
            var friendTwo = new Person { PersonId = 5, FirstName = "Drew", LastName = "Smith" };
            theHost.MyFriends = new List<Person>{friendOne, friendTwo};

            var controller = new EventController(_eventRepo, _personRepo, _eventService, _userService);

            //Act
            A.CallTo(() => _eventRepo.GetAll()).Returns(new List<Event> { dataModel }.AsQueryable());
            A.CallTo(() => _personRepo.GetAll()).Returns(new List<Person> { theHost }.AsQueryable());
            A.CallTo(() => _userService.GetCurrentUserId("")).Returns(1);//the username will be blank in unit tests... easier than mocking a ton of stuff

            var result = controller.Edit(1) as ViewResult;

            //Assert
            Assert.AreEqual(((EventViewModel)result.Model).Title, dataModel.Title);
            Assert.IsTrue(((EventViewModel)result.Model).PeopleList.Count == 2);
            Assert.AreEqual(result.ViewBag.StatusMessage, string.Empty);
        }

        /// <summary>
        /// This unit test ensures that all of the required fields for the event view model must be filled out on edit.
        /// </summary>
        [TestMethod]
        public void Edit_Event_ModelState_Not_Valid()
        {
            //Arrange
            var viewModel = new EventViewModel();
            var contoller = new EventController(_eventRepo, _personRepo, _eventService, _userService);

            var modelBinder = new ModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => viewModel, viewModel.GetType()),
                ValueProvider = new NameValueCollectionValueProvider(new NameValueCollection(), CultureInfo.InvariantCulture)
            };

            var binder = new DefaultModelBinder().BindModel(new ControllerContext(), modelBinder);
            contoller.ModelState.Clear();
            contoller.ModelState.Merge(modelBinder.ModelState);
            //Act
            var result = contoller.Edit(viewModel) as ViewResult;

            //Assert
            Assert.AreEqual(result.ViewData.ModelState.Count, 4);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        /// <summary>
        /// This unit test ensures that the correct message is returned in the query string if editing event succeeds.
        /// </summary>
        [TestMethod]
        public void Edit_Event_Success()
        {
            //Arrange
            var viewModel = GetTestEventViewModel(1);
            var expectedDataModel = GetTestEventDataModel(1);
            var contoller = new EventController(_eventRepo, _personRepo, _eventService, _userService);

            //Act
            A.CallTo(() => _eventRepo.GetAll()).Returns(new List<Event> { expectedDataModel }.AsQueryable());
            var result = contoller.Edit(viewModel) as RedirectToRouteResult;

            //Assert
            A.CallTo(() => _eventRepo.SubmitChanges()).MustHaveHappened();

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
            var contoller = new EventController(_eventRepo, _personRepo, _eventService, _userService);

            //Act
            A.CallTo(() => _eventRepo.GetAll()).Throws(new Exception("Database error!"));
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
            var contoller = new EventController(_eventRepo, _personRepo, _eventService, _userService);

            //Act
            A.CallTo(() => _eventRepo.GetAll()).Returns(new List<Event>{expectedDataModel}.AsQueryable());
            var result = contoller.Delete(1) as RedirectToRouteResult;

            //Assert
            A.CallTo(() => _eventRepo.Delete(A<Event>.That.Matches(x => x == expectedDataModel))).MustHaveHappened();
            A.CallTo(() => _eventRepo.SubmitChanges()).MustHaveHappened();

            //That the route values are what we expect.
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "Home");
            Assert.AreEqual(result.RouteValues["message"].ToString(), "DeleteSuccessful");
        }

        #region Helpers

        /// <summary>
        /// Get an event view model for testing purposes
        /// </summary>
        /// <param name="id">The id of the view model (default = 0)</param>
        private EventViewModel GetTestEventViewModel(int id = 0)
        {
            //People
            var theHost = new PersonViewModel
            {
                PersonId = 1,
                UserName = "jsmith",
                FirstName = "Joe",
                LastName = "Smith"
            };
            var guestOne = new PersonViewModel
            {
                PersonId = 2,
                UserName = "bbufford",
                FirstName = "Ben",
                LastName = "Bufford"
            };
            var guestTwo = new PersonViewModel
            {
                PersonId = 3,
                UserName = "shart",
                FirstName = "Sally",
                LastName = "Hart"
            };
            var theInvitees = new List<PersonViewModel> { guestOne, guestTwo };

            //Food
            var burgers = new FoodItemViewModel
            {
                FoodItemId = 1,
                Title = "Hambergers",
                Description = "Apple bacon smoked burgers for 10 people."
            };
            var coke = new FoodItemViewModel { FoodItemId = 2, Title = "Coke", Description = "Two 6 packs" };
            var foodForTheParty = new List<FoodItemViewModel> { burgers, coke };

            //Games
            var settlers = new GameViewModel
            {
                GameId = 1,
                Title = "Settlers of Catan",
                Description = "The best game ever for up to four people"
            };
            var blockus = new GameViewModel
            {
                GameId = 2,
                Title = "Blockus",
                Description = "Fun game of shape fitting for up four people."
            };
            var gamesForTheParty = new List<GameViewModel> { settlers, blockus };
            var viewModel = new EventViewModel
            {
                EventId = id,
                Title = "My Test Event",
                Description = "This is a fun test event",
                Coordinator = theHost,
                StartDate = DateTime.Now,
                StartTime = DateTime.Now.Date.AddHours(17),
                EndTime = DateTime.Now.AddHours(26),
                FoodItems = foodForTheParty,
                Games = gamesForTheParty,
                PeopleInvited = theInvitees
            };
            return viewModel;
        }

        /// <summary>
        /// Get an event data model for testing purposes
        /// </summary>
        /// <param name="id">The event id for the data model (defaul = 0)</param>
        /// <returns></returns>
        private Event GetTestEventDataModel(int id = 0)
        {
            var viewModel = GetTestEventViewModel(id);
            var dataModel = viewModel.GetDataModel();
            _eventService.SetEventDates(dataModel, viewModel);
            _eventService.AppendNewFoodItems(dataModel, viewModel);
            _eventService.AppendNewGames(dataModel, viewModel);
            _eventService.InviteNewPeople(dataModel, viewModel);
            return dataModel;
        }

        #endregion
    }
}
