using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
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
        /// This unit test ensures that the correct message is returned in the query string if creating event succeeds.
        /// </summary>
        [TestMethod]
        public void Create_Event_Success()
        {
            //Arrange

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
            var coke = new FoodItemViewModel { FoodItemId = 2, Title = "Coke", Description = "Two 6 packs"};
            var foodForTheParty = new List<FoodItemViewModel> { burgers, coke };

            //Games
            var settlers = new GameViewModel { GameId = 1, Title = "Settlers of Catan", Description = "The best game ever for up to four people"};
            var blockus = new GameViewModel { GameId = 2, Title= "Blockus", Description = "Fun game of shape fitting for up four people."};
            var gamesForTheParty = new List<GameViewModel> { settlers, blockus };
            var viewModel = new EventViewModel
                {
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

            //Data model result
            var expectedDataModel = viewModel.GetDataModel();
            _eventService.SetEventDates(expectedDataModel, viewModel);
            _eventService.AppendNewFoodItems(expectedDataModel, viewModel);
            _eventService.AppendNewGames(expectedDataModel, viewModel);
            _eventService.InviteNewPeople(expectedDataModel, viewModel);

            var contoller = new EventController(_eventRepo, _personRepo, _eventService, _userService);

            //Act
            var result = contoller.Create(viewModel) as RedirectToRouteResult;

            //Assert
            //That the data model being passed to the repository is what we expect.
            A.CallTo(() => _eventRepo.Insert(A<Event>.That.Matches(x => x != expectedDataModel))).MustHaveHappened();
            A.CallTo(() => _eventRepo.SubmitChanges()).MustHaveHappened();

            //Tthat the route values are what we expect.
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "Home");
            Assert.AreEqual(result.RouteValues["message"].ToString(), "SaveModelSuccess");
        }

        /// <summary>
        /// This unit test ensures that all of the required fields for the event view model must be filled out on edit.
        /// </summary>
        [TestMethod]
        public void Edit_Event_Model_Not_Valid()
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
            Assert.IsFalse(result.ViewData.ModelState.Count == 7);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
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
