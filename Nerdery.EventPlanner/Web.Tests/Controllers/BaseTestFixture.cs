using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Data;
using Web.Data.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.Tests.Controllers
{
    [TestClass]
    public class BaseTestFixture
    {
        #region Fields

        protected IRepository<Person> PersonRepo;
        protected IRepository<Event> EventRepo;
        protected IRepository<FoodItem> FoodRepo;
        protected IRepository<Game> GameRepo;
        protected IUserService UserService;
        protected IEventService EventService;
        protected INotificationService NotificationService;

        [TestInitialize]
        public void SpinUp()
        {
            PersonRepo = A.Fake<IRepository<Person>>();
            EventRepo = A.Fake<IRepository<Event>>();
            FoodRepo = A.Fake<IRepository<FoodItem>>();
            GameRepo = A.Fake<IRepository<Game>>();
            UserService = A.Fake<IUserService>();
            EventService = new EventService(PersonRepo, GameRepo, FoodRepo);
            NotificationService = new NotificationService(PersonRepo, EventRepo);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get an event view model for testing purposes
        /// </summary>
        /// <param name="id">The id of the view model (default = 0)</param>
        protected EventViewModel GetTestEventViewModel(int id = 0)
        {
            //People
            var theHost = 1;
            var guestOne = 2;
            var guestTwo = 3;
            var theInvitees = new List<int> { guestOne, guestTwo };

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
                StartDate = DateTime.Now,
                StartTime = "5:00 PM",
                EndTime = "2:00 AM",
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
        protected Event GetTestEventDataModel(int id = 0)
        {
            var viewModel = GetTestEventViewModel(id);
            var dataModel = viewModel.GetDataModel();
            EventService.SetEventDates(dataModel, viewModel);
            EventService.AppendNewFoodItems(dataModel, viewModel);
            EventService.AppendNewGames(dataModel, viewModel);
            EventService.InviteNewPeople(dataModel, viewModel);
            return dataModel;
        }

        protected PersonViewModel GetTestInviteeViewModel(int id = 0)
        {
            var iAmInvited = new PersonViewModel
                {
                    PersonId = id,
                    FirstName = "Sam",
                    LastName = "Mercuiou",
                };

            //Build a list of food items...
            iAmInvited.MyFoodItems.Add(GetTestFoodItemViewModel(4));
            iAmInvited.MyFoodItems.Add(GetTestFoodItemViewModel(5));

            //Build list of games...
            iAmInvited.MyGames.Add(GetTestGameViewModel(4));
            iAmInvited.MyGames.Add(GetTestGameViewModel(5));

            return iAmInvited;
        }

        protected Person GetTestInviteeDataModel(int id = 0)
        {
            return GetTestInviteeViewModel(id).GetDataModel();
        }

        protected PersonViewModel GetTestFriend(int id = 0)
        {
            var friend = new PersonViewModel
                {
                    PersonId = id,
                    FirstName = "Best",
                    LastName = "Friend"
                };

            return friend;
        }

        protected FoodItemViewModel GetTestFoodItemViewModel(int id = 0)
        {
            var viewModel = new FoodItemViewModel
            {
                FoodItemId = id,
                Title = "Yummy",
                Description = "Seriously, this is amazing!"
            };

            return viewModel;
        }

        protected FoodItem GetTestFoodItemDataModel(int id)
        {
            return GetTestFoodItemViewModel(id).GetDataModel();
        }

        protected GameViewModel GetTestGameViewModel(int id = 0)
        {
            var viewModel = new GameViewModel
                {
                    GameId = id,
                    Title = "A really cool game",
                    Description = "Seriously, this is an awesome game"
                };

            return viewModel;
        }

        protected Game GetTestGameDataModel(int id)
        {
            return GetTestGameViewModel(id).GetDataModel();
        }

        #endregion
    }
}
