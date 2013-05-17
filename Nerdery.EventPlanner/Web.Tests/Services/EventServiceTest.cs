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
using Web.Tests.Controllers;
using Web.ViewModels;

namespace Web.Tests.Services
{
    public class EventServiceTest : BaseTestFixture
    {
        /// <summary>
        /// This test ensures that the start date for an event is parsed together correctly from the parameter
        /// values that would be supplied from a view model
        /// </summary>
        [TestMethod]
        public void Parse_Event_Dates()
        {
            //Arrange
            var hours = 4;
            var startTimeValue = "4:00 AM"; //today
            var endTimeValue = "2:30 AM"; //This SHOULD be 2AM the next day...
            var dataModel = new Event();
            var viewModel = new EventViewModel { StartDate = DateTime.Now, StartTime = startTimeValue, EndTime = endTimeValue };

            //Act
            EventService.SetEventDates(dataModel, viewModel);

            //Assert
            Assert.AreEqual(dataModel.StartDate, DateTime.Now.Date.AddHours(4));
            Assert.AreEqual(dataModel.EndDate, DateTime.Now.Date.AddHours(26).AddMinutes(30));
        }
        /// <summary>
        /// This unit test will ensure that people can be invited to an event
        /// </summary>
        [TestMethod]
        public void Invite_New_People()
        {
            //Arrange
            var personOne = 1;
            var personTwo = 2;
            var personThree = 3;
            var viewModel = new EventViewModel{ PeopleInvited = new List<int>{personTwo, personThree} };
            var dataModel = new Event
                {
                    PeopleInvited = new List<Person> {new Person{PersonId = 2}, new Person{PersonId = 3}}
                };

            //Act
            viewModel.PeopleInvited.Add(personOne);
            EventService.InviteNewPeople(dataModel, viewModel);

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
            var personOne = 1;
            var personTwo = 2;
            var personThree = 3;
            var viewModel = new EventViewModel { PeopleInvited = new List<int> { personTwo, personThree } };
            var dataModel = new Event
            {
                PeopleInvited = new List<Person> { new Person{PersonId = 2}, new Person{PersonId = 3} },
                PeopleWhoAccepted = new List<Person> { new Person { PersonId = 2 } },
                PeopleWhoDeclined = new List<Person> { new Person { PersonId = 3 } }
            };

            //Act
            viewModel.PeopleInvited.Remove(personThree);
            viewModel.PeopleInvited.Remove(personTwo);
            EventService.UninvitePeople(dataModel, viewModel);

            //Assert
            Assert.AreEqual(dataModel.PeopleInvited.Count, 0);
            Assert.AreEqual(dataModel.PeopleWhoAccepted.Count, 0);
            Assert.AreEqual(dataModel.PeopleWhoDeclined.Count, 0);            
        }
        /// <summary>
        /// Get a list of am / pm friendly times for the UI
        /// </summary>
        [TestMethod]
        public void Get_Time_List()
        {
            var timeList = EventService.GetTimeList();

            Assert.AreEqual(timeList.Count, 96);
        }
    }
}
