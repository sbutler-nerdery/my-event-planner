﻿using System;
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
    [TestClass]
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
            var viewModel = new EditEventViewModel { StartDate = DateTime.Now, StartTime = startTimeValue, EndTime = endTimeValue };

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
            var personOne = "1";
            var personTwo = "2";
            var personThree = "3";
            var viewModel = new EditEventViewModel{ PeopleInvited = new List<string>{personTwo, personThree} };
            var dataModel = new Event
                {
                    RegisteredInvites = new List<Person> {new Person{PersonId = 2}, new Person{PersonId = 3}},
                    NonRegisteredInvites = new List<PendingInvitation>()
                };

            A.CallTo(() => PersonRepo.GetAll()).Returns(new List<Person> { new Person() { PersonId = 1 } }.AsQueryable());

            //Act
            viewModel.PeopleInvited.Add(personOne);
            EventService.InviteNewPeople(dataModel, viewModel);

            //Assert
            Assert.AreEqual(dataModel.RegisteredInvites.Count, 3);
        }
        [TestMethod]
        public void Invite_New_People_By_Email()
        {
            //Arrange
            var ben = "ben@email.com|Ben|Bufford";
            var dan = "dan@email.com|Dan|Gidman";
            var herb = "herb@email.com|Herb|Nease";
            var viewModel = new EditEventViewModel { PeopleInvited = new List<string> { dan, herb } };
            var dataModel = new Event
            {
                Coordinator = new Person { PersonId = 1, MyNonRegisteredFriends = new List<PendingInvitation>()},
                NonRegisteredInvites = new List<PendingInvitation> { new PendingInvitation { Email = "dan@email.com" }, new PendingInvitation { Email = "herb@email.com" } },
                RegisteredInvites = new List<Person>()
            };

            A.CallTo(() => InvitationRepo.GetAll()).Returns(new List<PendingInvitation> { new PendingInvitation { Email = "ben@email.com" } }.AsQueryable());

            //Act
            viewModel.PeopleInvited.Add(ben);
            EventService.InviteNewPeople(dataModel, viewModel);

            //Assert
            Assert.AreEqual(dataModel.NonRegisteredInvites.Count, 3);

        }
        [TestMethod]
        public void Invite_New_People_By_Facebook()
        {
            //Arrange
            var ben = "00000|Ben Van Orm Bufford";
            var dan = "11111|Dan Gidman";
            var herb = "22222|Herb Nease";
            var viewModel = new EditEventViewModel { PeopleInvited = new List<string> { dan, herb } };
            var dataModel = new Event
            {
                Coordinator = new Person { PersonId = 1, MyNonRegisteredFriends = new List<PendingInvitation>()},
                NonRegisteredInvites = new List<PendingInvitation> { new PendingInvitation { FacebookId = "11111" }, new PendingInvitation { FacebookId = "22222" } },
                RegisteredInvites = new List<Person>()
            };

            A.CallTo(() => InvitationRepo.GetAll()).Returns(new List<PendingInvitation> { new PendingInvitation { FacebookId = "00000" } }.AsQueryable());

            //Act
            viewModel.PeopleInvited.Add(ben);
            EventService.InviteNewPeople(dataModel, viewModel);

            //Assert
            Assert.AreEqual(dataModel.NonRegisteredInvites.Count, 3);
        }
        /// <summary>
        /// This unit test will ensure that people can be un-invited to an event
        /// </summary>
        [TestMethod]
        public void Uninvite_People()
        {
            //Arrange
            var personOne = "1";
            var personTwo = "2";
            var personThree = "3";
            var emailPerson = "bart@email.com|Bart|Simpson";
            var facebookPerson = "00000|Homer Simpson";
            var viewModel = new EditEventViewModel { PeopleInvited = new List<string> { personTwo, personThree, emailPerson, facebookPerson } };
            var dataModel = new Event
            {
                RegisteredInvites = new List<Person> { new Person { PersonId = 2 }, new Person { PersonId = 3 } },
                NonRegisteredInvites = new List<PendingInvitation> { new PendingInvitation { Email = "bart@email.com" }, new PendingInvitation { FacebookId = "00000" } },
                PeopleWhoAccepted = new List<Person> { new Person { PersonId = 2 } },
                PeopleWhoDeclined = new List<Person> { new Person { PersonId = 3 } }
            };

            //Act
            viewModel.PeopleInvited.Remove(personThree);
            viewModel.PeopleInvited.Remove(personTwo);
            viewModel.PeopleInvited.Remove(emailPerson);
            viewModel.PeopleInvited.Remove(facebookPerson);
            EventService.UninvitePeople(dataModel, viewModel);

            //Assert
            Assert.AreEqual(dataModel.RegisteredInvites.Count, 0);
            Assert.AreEqual(dataModel.NonRegisteredInvites.Count, 0);
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
        /// <summary>
        /// Make sure we get a model state that is not blank.
        /// </summary>
        [TestMethod]
        public void Get_Event_Model_State()
        {
            var model = new Event
                {
                    Title = "My title", 
                    Description = "Test description", 
                    Location = "My House", 
                    StartDate = DateTime.Now, 
                    EndDate = DateTime.Now.AddHours(3),
                    NonRegisteredInvites = new List<PendingInvitation>(),
                    RegisteredInvites = new List<Person>()
                };

            var modelState = EventService.GetSerializedModelState(model);

            Assert.AreNotEqual(modelState, string.Empty);
        }
        /// <summary>
        /// Make sure that the service will correctly determine which users are newly invited.
        /// </summary>
        [TestMethod]
        public void Get_Newly_Invited_Registered_users()
        {
            //Arrange
            var peopleInvited = new List<Person>
                {
                    new Person {PersonId = 1}
                };

            var currentPeopleInvited = new List<Person>
                {
                    new Person {PersonId = 1},
                    new Person {PersonId = 2},
                    new Person {PersonId = 3},
                };

            //Act
            var list = EventService.GetRegisteredInvites(peopleInvited, currentPeopleInvited);

            //Assert
            Assert.AreEqual(list.Count, 2);
        }
        /// <summary>
        /// Get a list of people who were uninvited from an event.
        /// </summary>
        [TestMethod]
        public void Get_Uninvited_Registered_Users()
        {
            //Arrange
            var peopleInvited = new List<Person>
                {
                    new Person {PersonId = 1},
                    new Person {PersonId = 2},
                    new Person {PersonId = 3},
                };

            var currentPeopleInvited = new List<Person>
                {
                    new Person {PersonId = 1}
                };

            //Act
            var list = EventService.GetRegisteredUninvites(peopleInvited, currentPeopleInvited);

            //Assert
            Assert.AreEqual(list.Count, 2);            
        }
    }
}
