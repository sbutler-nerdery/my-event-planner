using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Web.Data;
using Web.Data.Models;
using Web.ViewModels;

namespace Web.Services
{
    public class EventService : IEventService
    {
        #region Fields

        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<Person> _personPersonRepo;
        private readonly IRepository<Game> _gameRepository;
        private readonly IRepository<FoodItem> _foodRepository;
        private readonly IRepository<PendingInvitation> _invitationRepository;

        #endregion

        #region Constructors

        public EventService(IRepositoryFactory factory)
        {
            _personPersonRepo = factory.GetRepository<Person>();
            _gameRepository = factory.GetRepository<Game>();
            _foodRepository = factory.GetRepository<FoodItem>();
            _invitationRepository = factory.GetRepository<PendingInvitation>();
        }

        #endregion

        #region Methods

        public List<string> GetTimeList()
        {
            var timeList = new List<string>();

            //Build the time list
            for (int hour = 0; hour < 24; hour++)
            {
                var hourString = FormatHour(hour);
                var ampm = (hour < 12) ? "AM" : "PM";
                for (int minute = 0; minute < 60; minute += 15)
                {
                    var minuteString = (minute == 0) ? "00" : minute.ToString();
                    timeList.Add(string.Format("{0}:{1} {2}", hourString, minuteString, ampm));
                }
            }

            return timeList;
        }

        public void SetEventDates(Event dataModel, EditEventViewModel viewModel)
        {
            DateTime startTime =
                DateTime.Parse(viewModel.StartDate.Value.ToShortDateString() + " " + viewModel.StartTime);
            DateTime endTime =
                DateTime.Parse(viewModel.StartDate.Value.ToShortDateString() + " " + viewModel.EndTime);

            int hours = startTime.Hour;
            int minutes = startTime.Minute;

            //Set the data model start date...
            dataModel.StartDate = viewModel.StartDate.Value.Date.AddHours(hours).AddMinutes(minutes);

            int endHour = endTime.Hour;
            int endMinute = endTime.Minute;
            dataModel.EndDate = dataModel.StartDate.Date.AddHours(endHour).AddMinutes(endMinute);

            //Change the end date if...
            if (dataModel.StartDate.Hour > endTime.Hour)
                dataModel.EndDate = dataModel.StartDate.Date.AddDays(1).Date.AddHours(endHour).AddMinutes(endMinute);
        }

        public void AppendNewFoodItems(Event dataModel, EventBaseViewModel viewModel)
        {
            var dataFoodIds = dataModel.FoodItems.Select(x => x.FoodItemId.ToString()).ToArray(); //Items in the database
            var newFoodItems = viewModel.WillBringTheseFoodItems.Where(x => !dataFoodIds.Contains(x)).ToList();

            //Add new items
            newFoodItems.ForEach(foodId =>
                {
                    var id = int.Parse(foodId);
                    var addMe = _foodRepository.GetAll().FirstOrDefault(y => y.FoodItemId == id);

                    if (addMe != null)
                        dataModel.FoodItems.Add(addMe);
                });
        }

        public void RemoveFoodItems(Event dataModel, EventBaseViewModel viewModel)
        {
            var hostFoodIds = viewModel.WillBringTheseFoodItems.Select(x => x).ToArray();
            var thePerson = _personPersonRepo.GetAll().FirstOrDefault(x => x.PersonId == viewModel.PersonId);
            var deletedFoodItemIds = thePerson.MyFoodItems.Where(x => !hostFoodIds.Contains(x.FoodItemId.ToString())).Select(x => x.FoodItemId).ToList();

            //Delete items
            deletedFoodItemIds.ForEach(id =>
            {
                var removeMe = dataModel.FoodItems.FirstOrDefault(y => y.FoodItemId == id);

                if (removeMe != null)
                    dataModel.FoodItems.Remove(removeMe);
            });
        }

        public void AppendNewGames(Event dataModel, EventBaseViewModel viewModel)
        {
            var dataGameIds = dataModel.Games.Select(x => x.GameId).ToArray(); //Items in the database
            var newGameItems = viewModel.WillBringTheseGames.Where(x => !dataGameIds.Contains(int.Parse(x))).ToList();

            //Add new items
            newGameItems.ForEach(gameId =>
                {
                    var id = int.Parse(gameId);
                    var addMe = _gameRepository.GetAll().FirstOrDefault(y => y.GameId == id);

                    if (addMe != null)
                        dataModel.Games.Add(addMe);
                });
        }

        public void RemoveGames(Event dataModel, EventBaseViewModel viewModel)
        {
            var hostGameIds = viewModel.WillBringTheseGames.Select(x => x).ToArray(); //Items in local view model
            var thePerson = _personPersonRepo.GetAll().FirstOrDefault(x => x.PersonId == viewModel.PersonId);
            var deletedGameIds = thePerson.MyGames.Where(x => !hostGameIds.Contains(x.GameId.ToString())).Select(x => x.GameId).ToList();

            //Delete items
            deletedGameIds.ForEach(id =>
            {
                var removeMe = dataModel.Games.FirstOrDefault(y => y.GameId == id);

                if (removeMe != null)
                    dataModel.Games.Remove(removeMe);
            });
        }

        public void InviteNewPeople(Event dataModel, EditEventViewModel viewModel)
        {
            int tParse;

            //Add the temp user ids to the pending invitations table
            var emailList = new List<string>();
            var facebookIdList = new List<string>();
            var emailInvites = viewModel.PeopleInvited.Where(x => x.Split('|').Length == 3).ToList();
            var facebookInvites = viewModel.PeopleInvited.Where(x => x.Split('|').Length == 2).ToList();

            emailInvites.ForEach(x =>
                {
                    var tempUserArray = x.Split('|');
                    var emailAddress = tempUserArray[0];
                    emailList.Add(emailAddress);

                    //Make sure it doesn't exist already
                    var exists = _invitationRepository.GetAll().FirstOrDefault(y => y.Email == emailAddress);

                    if (exists == null)
                    {
                        var emailInvite = new PendingInvitation
                        {
                            PersonId = dataModel.Coordinator.PersonId,
                            Email = emailAddress,
                            FirstName = tempUserArray[1],
                            LastName = tempUserArray[2]
                        };

                        _invitationRepository.Insert(emailInvite);
                    }
                });

            facebookInvites.ForEach(x =>
            {
                var tempUserArray = x.Split('|');
                var facebookId = tempUserArray[0];

                //Get the first and last name values
                var nameArray = tempUserArray[1].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var firstName = nameArray[0];
                var lastName = nameArray[nameArray.Length - 1];

                facebookIdList.Add(facebookId);

                //Make sure it doesn't exist already
                var exists = _invitationRepository.GetAll().FirstOrDefault(y => y.FacebookId == facebookId);

                if (exists == null)
                {
                    var facebookInvite = new PendingInvitation
                    {
                        PersonId = dataModel.Coordinator.PersonId,
                        FacebookId = facebookId,
                        FirstName = firstName,
                        LastName = lastName
                    };

                    _invitationRepository.Insert(facebookInvite);
                }
            });

            _invitationRepository.SubmitChanges();

            //Find the existing user ids
            var dataPeopleIds = dataModel.RegisteredInvites.Select(x => x.PersonId).ToArray(); //Items in the database
            var newPeople = viewModel.PeopleInvited
                .Where(x => int.TryParse(x, out tParse)) //only get the values that parse as ints
                .Where(x => !dataPeopleIds.Contains(int.Parse(x))).ToList();

            //Add new people
            newPeople.ForEach(personId =>
                {
                    var id = int.Parse(personId);
                    var inviteMe = _personPersonRepo.GetAll().FirstOrDefault(person => person.PersonId == id);

                    if (inviteMe != null)
                        dataModel.RegisteredInvites.Add(inviteMe);
                });

            //Find the existing temp emails
            var emailPeople = dataModel.NonRegisteredInvites.Where(x => emailList.Contains(x.Email))
                .Select(x => x.Email).ToArray(); //Items in the database
            var newEmailPeople = emailList
                .Where(x => !emailPeople.Contains(x)).ToList();

            //Add people invited by email
            newEmailPeople.ForEach(email =>
            {
                var inviteMe = _invitationRepository.GetAll().FirstOrDefault(invite => invite.Email == email);

                if (inviteMe != null)
                {
                    dataModel.NonRegisteredInvites.Add(inviteMe);

                    //Add the new email invite to the user's list of friends if necissary...
                    var exists =
                        dataModel.Coordinator.MyNonRegisteredFriends.FirstOrDefault(
                            x => x.PendingInvitationId == inviteMe.PendingInvitationId);

                    if (exists == null)
                    {
                        dataModel.Coordinator.MyNonRegisteredFriends.Add(inviteMe);
                    }
                }
            });

            //Find the existing temp facebook ids
            var facebookPeople = dataModel.NonRegisteredInvites.Where(x => facebookIdList.Contains(x.FacebookId))
                .Select(x => x.FacebookId).ToArray(); //Items in the database
            var newFacebookPeople = facebookIdList
                .Where(x => !facebookPeople.Contains(x)).ToList();

            //Add new people invited by facebook
            newFacebookPeople.ForEach(facebookId =>
            {
                var inviteMe = _invitationRepository.GetAll().FirstOrDefault(invite => invite.FacebookId == facebookId);

                if (inviteMe != null)
                {
                    dataModel.NonRegisteredInvites.Add(inviteMe);

                    //Add the new facebook invite to the user's list of friends if necissary...
                    var exists =
                        dataModel.Coordinator.MyNonRegisteredFriends.FirstOrDefault(
                            x => x.PendingInvitationId == inviteMe.PendingInvitationId);

                    if (exists == null)
                    {
                        dataModel.Coordinator.MyNonRegisteredFriends.Add(inviteMe);
                    }
                }
            });
        }

        public void UninvitePeople(Event dataModel, EditEventViewModel viewModel)
        {
            int parse;
            //Process peoeple with user accounts
            var modelPeopleIds = viewModel.PeopleInvited
                .Where(x => int.TryParse(x, out parse))
                .Select(int.Parse).ToArray(); //Items in local view model
            var deletedPeopleIds = dataModel.RegisteredInvites.Where(x => !modelPeopleIds.Contains(x.PersonId)).Select(x => x.PersonId).ToList();

            //Delete items
            deletedPeopleIds.ForEach(id =>
                {
                    var thePerson = _personPersonRepo.GetAll().FirstOrDefault(x => x.PersonId == id);
                    var removeInvitation = dataModel.RegisteredInvites.FirstOrDefault(y => y.PersonId == id);
                    var removeAccepted = dataModel.PeopleWhoAccepted.FirstOrDefault(y => y.PersonId == id);
                    var removeDeclined = dataModel.PeopleWhoDeclined.FirstOrDefault(y => y.PersonId == id);

                    //Remove from the invitation list
                    dataModel.RegisteredInvites.Remove(removeInvitation);

                    //Remove from the accepted list
                    if (removeAccepted != null)
                        dataModel.PeopleWhoAccepted.Remove(removeAccepted);

                    //Remove from the declined list
                    if (removeDeclined != null)
                        dataModel.PeopleWhoDeclined.Remove(removeDeclined);

                    //Remove the person's food items from the event
                    thePerson.MyFoodItems.ForEach(x =>
                        {
                            var removeMe = dataModel.FoodItems.FirstOrDefault(y => y.FoodItemId == x.FoodItemId);

                            if (removeMe != null)
                                dataModel.FoodItems.Remove(removeMe);
                        });

                    //Remove the person's games from the event
                    thePerson.MyGames.ForEach(x =>
                    {
                        var removeMe = dataModel.Games.FirstOrDefault(y => y.GameId == x.GameId);

                        if (removeMe != null)
                            dataModel.Games.Remove(removeMe);
                    });

                });

            //Process people without user accounts
            var emailList = new List<string>();
            var facebookIdList = new List<string>();
            viewModel.PeopleInvited.Where(x => x.Split('|').Length == 3).ToList().ForEach(x =>
            {
                var tempArray = x.Split('|');
                emailList.Add(tempArray[0]);
            });
            viewModel.PeopleInvited.Where(x => x.Split('|').Length == 2).ToList().ForEach(x =>
            {
                var tempArray = x.Split('|');
                facebookIdList.Add(tempArray[0]);
            });

            var deletedEmailInvites = dataModel.NonRegisteredInvites
                .Where(x => !emailList.Contains(x.Email) && x.FacebookId == null)
                .Select(x => x.Email).ToList();
            var deletedFacebookInvites = dataModel.NonRegisteredInvites
                .Where(x => !facebookIdList.Contains(x.FacebookId) && x.Email == null)
                .Select(x => x.FacebookId).ToList();

            deletedEmailInvites.ForEach(email =>
            {
                var removeInvitation = dataModel.NonRegisteredInvites.FirstOrDefault(y => y.Email == email);

                //Remove from the invitation list
                dataModel.NonRegisteredInvites.Remove(removeInvitation);
            });

            deletedFacebookInvites.ForEach(facebookId =>
            {
                var removeInvitation = dataModel.NonRegisteredInvites.FirstOrDefault(y => y.FacebookId == facebookId);

                //Remove from the invitation list
                dataModel.NonRegisteredInvites.Remove(removeInvitation);
            });
        }

        public string GetSerializedModelState(Event dataModel)
        {
            var modelState =
                new
                {
                    dataModel.Title,
                    dataModel.Description,
                    dataModel.Location,
                    dataModel.StartDate
                };

            return JsonConvert.SerializeObject(modelState);
        }

        public List<Person> GetRegisteredInvites(List<Person> previousInvites, List<Person> currentInvites)
        {
            var oldPeopleIds = previousInvites.Select(x => x.PersonId).ToArray(); //Items in the database
            var newPeople = currentInvites
                .Where(x => !oldPeopleIds.Contains(x.PersonId)).ToList();
            return newPeople;
        }

        public List<PendingInvitation> GetNonRegisteredInvites(List<PendingInvitation> previousInvites, List<PendingInvitation> currentInvites)
        {
            var oldPeopleIds = previousInvites.Select(x => x.PendingInvitationId).ToArray(); //Items in the database
            var newPeople = currentInvites
                .Where(x => !oldPeopleIds.Contains(x.PendingInvitationId)).ToList();
            return newPeople;
        }

        public List<Person> GetRegisteredUninvites(List<Person> previousInvites, List<Person> currentInvites)
        {
            var currentPeopleIds = currentInvites.Select(x => x.PersonId).ToArray(); //Items in the database
            var uninviteThesePeople = previousInvites
                .Where(x => !currentPeopleIds.Contains(x.PersonId)).ToList();
            return uninviteThesePeople;
        }

        public List<PendingInvitation> GetNonRegisteredUninvites(List<PendingInvitation> previousInvites, List<PendingInvitation> currentInvites)
        {
            var currentPendingIds = currentInvites.Select(x => x.PendingInvitationId).ToArray(); //Items in the database
            var uninviteThesePeople = previousInvites
                .Where(x => !currentPendingIds.Contains(x.PendingInvitationId)).ToList();
            return uninviteThesePeople;
        }

        /// <summary>
        /// Format a numeric hour (in military time) to a string
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        private string FormatHour(int hour)
        {
            //If the number is greater then 12 then subtract 12 from it to get AM / PM friendly values.
            if (hour >= 12)
                hour -= 12;

            if (hour == 0)
                return "12";

            return hour.ToString();
        }

        #endregion
    }
}