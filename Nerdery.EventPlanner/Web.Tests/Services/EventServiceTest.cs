using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Data.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.Tests.Services
{
    [TestClass]
    public class EventServiceTest
    {
        private IEventService _eventService;

        [TestInitialize]
        public void SpinUp()
        {
            _eventService = new EventService();
        }

        /// <summary>
        /// This test ensures that the start date for an event is parsed together correctly from the parameter
        /// values that would be supplied from a view model
        /// </summary>
        [TestMethod]
        public void Create_Event_Parse_Start_Date()
        {
            var startDateValue = DateTime.Now;
            var startTimeValue = DateTime.Now.Date.AddHours(4).AddMinutes(30); //4:30 AM today
            var compiledStartDate = _eventService.GetEventStartDate(startDateValue, startTimeValue);
            Assert.AreEqual(compiledStartDate, startTimeValue);
        }
        /// <summary>
        /// This test ensures that if the end time in the view model is past midnight the end date of the data model
        /// is adjusted to the next day properly.
        /// </summary>
        [TestMethod]
        public void Create_Event_Handle_Past_Midnight()
        {
            //Arrange
            var startDate = DateTime.Now.Date.AddHours(19); //7PM
            var endDate = DateTime.Now.Date.AddHours(2); //This SHOULD be 2AM the next day...

            //Act
            var modifiedEndDate = _eventService.GetEventEndDate(startDate, endDate);

            //Assert that the new date is 2AM the next day
            Assert.AreEqual(DateTime.Now.Date.AddHours(26), modifiedEndDate);
        }
        /// <summary>
        /// This unit test will ensure that people can be invited to an event
        /// </summary>
        [TestMethod]
        public void Invite_New_People()
        {
            //Arrange
            var personOne = new PersonViewModel {PersonId = 1, FirstName = "Joe", LastName = "Smith"};
            var personTwo = new PersonViewModel {PersonId = 2, FirstName = "Sally", LastName = "Smith"};
            var personThree = new PersonViewModel {PersonId = 3, FirstName = "Joe", LastName = "Smith"};
            var viewModel = new EventViewModel{ PeopleInvited = new List<PersonViewModel>{personTwo, personThree} };
            var dataModel = new Event
                {
                    PeopleInvited = new List<Person> {personTwo.GetDataModel(), personThree.GetDataModel()}
                };

            //Act
            viewModel.PeopleInvited.Add(personOne);
            _eventService.InviteNewPeople(dataModel, viewModel);

            //Assert
            Assert.AreEqual(dataModel.PeopleInvited.Count, 3);
        }
        /// <summary>
        /// This unit test will ensure that people can be un-invited to an event
        /// </summary>
        [TestMethod]
        public void Uninvite_People()
        {
            //Arrange
            var personOne = new PersonViewModel { PersonId = 1, FirstName = "Joe", LastName = "Smith" };
            var personTwo = new PersonViewModel { PersonId = 2, FirstName = "Sally", LastName = "Smith" };
            var personThree = new PersonViewModel { PersonId = 3, FirstName = "Joe", LastName = "Smith" };
            var viewModel = new EventViewModel { PeopleInvited = new List<PersonViewModel> { personTwo, personThree } };
            var dataModel = new Event
            {
                PeopleInvited = new List<Person> { personTwo.GetDataModel(), personThree.GetDataModel() },
                PeopleWhoAccepted = new List<Person> { personTwo.GetDataModel() },
                PeopleWhoDeclined = new List<Person> { personThree.GetDataModel() }
            };

            //Act
            viewModel.PeopleInvited.Remove(personThree);
            viewModel.PeopleInvited.Remove(personTwo);
            _eventService.UninvitePeople(dataModel, viewModel);

            //Assert
            Assert.AreEqual(dataModel.PeopleInvited.Count, 0);
            Assert.AreEqual(dataModel.PeopleWhoAccepted.Count, 0);
            Assert.AreEqual(dataModel.PeopleWhoDeclined.Count, 0);            
        }
    }
}
