using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Data;
using Web.Data.Models;
using Web.ViewModels;

namespace Web.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository<Person> _personPersonRepo;
        private readonly IRepository<Game> _gameRepository;
        private readonly IRepository<FoodItem> _foodRepository;

        public EventService(IRepository<Person> personRepo, IRepository<Game> gameRepo, IRepository<FoodItem> foodRepo)
        {
            _personPersonRepo = personRepo;
            _gameRepository = gameRepo;
            _foodRepository = foodRepo;
        }

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

        public void SetEventDates(Event dataModel, EventViewModel viewModel)
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
            dataModel.EndDate = dataModel.StartDate.AddHours(endHour).AddMinutes(endMinute);

            //Change the end date if...
            if (dataModel.StartDate.Hour > endTime.Hour)
                dataModel.EndDate = dataModel.StartDate.AddDays(1).Date.AddHours(endHour).AddMinutes(endMinute); 
        }

        public void AppendNewFoodItems(Event dataModel, EventViewModel viewModel)
        {
            var dataFoodIds = dataModel.FoodItems.Select(x => x.FoodItemId).ToArray(); //Items in the database
            var newFoodItems = viewModel.FoodItems.Where(x => !dataFoodIds.Contains(x.FoodItemId)).ToList();

            //Add new items
            newFoodItems.ForEach(food =>
                {
                    var addMe = _foodRepository.GetAll().FirstOrDefault(y => y.FoodItemId == food.FoodItemId);
                    dataModel.FoodItems.Add(addMe);
                });
        }

        public void RemoveFoodItems(Event dataModel, EventViewModel viewModel)
        {
            var modelFoodIds = viewModel.FoodItems.Select(x => x.FoodItemId).ToArray(); //Items in local view model
            var deletedFoodItemIds = dataModel.FoodItems.Where(x => !modelFoodIds.Contains(x.FoodItemId)).Select(x => x.FoodItemId).ToList();

            //Delete items
            deletedFoodItemIds.ForEach(id =>
            {
                var removeMe = dataModel.FoodItems.FirstOrDefault(y => y.FoodItemId == id);
                dataModel.FoodItems.Remove(removeMe);
            });
        }

        public void AppendNewGames(Event dataModel, EventViewModel viewModel)
        {
            var dataGameIds = dataModel.Games.Select(x => x.GameId).ToArray(); //Items in the database
            var newGameItems = viewModel.Games.Where(x => !dataGameIds.Contains(x.GameId)).ToList();

            //Add new items
            newGameItems.ForEach(game =>
                {
                    var addMe = _gameRepository.GetAll().FirstOrDefault(y => y.GameId == game.GameId);
                    dataModel.Games.Add(addMe);
                });
        }

        public void RemoveGames(Event dataModel, EventViewModel viewModel)
        {
            var modelGameIds = viewModel.Games.Select(x => x.GameId).ToArray(); //Items in local view model
            var deletedGameIds = dataModel.Games.Where(x => !modelGameIds.Contains(x.GameId)).Select(x => x.GameId).ToList();

            //Delete items
            deletedGameIds.ForEach(id =>
            {
                var removeMe = dataModel.Games.FirstOrDefault(y => y.GameId == id);
                dataModel.Games.Remove(removeMe);
            });
        }

        public void InviteNewPeople(Event dataModel, EventViewModel viewModel)
        {
                var dataPeopleIds = dataModel.PeopleInvited.Select(x => x.PersonId).ToArray(); //Items in the database
                var newPeople = viewModel.PeopleInvited.Where(x => !dataPeopleIds.Contains(x)).ToList();

                //Add new people
                newPeople.ForEach(personId =>
                    {
                        var inviteMe = _personPersonRepo.GetAll().FirstOrDefault(person => person.PersonId == personId);
                        dataModel.PeopleInvited.Add(inviteMe);
                    });

        }

        public void UninvitePeople(Event dataModel, EventViewModel viewModel)
        {
                var modelPeopleIds = viewModel.PeopleInvited.Select(x => x).ToArray(); //Items in local view model
                var deletedPeopleIds = dataModel.PeopleInvited.Where(x => !modelPeopleIds.Contains(x.PersonId)).Select(x => x.PersonId).ToList();

                //Delete items
                deletedPeopleIds.ForEach(id =>
                {
                    var removeInitation = dataModel.PeopleInvited.FirstOrDefault(y => y.PersonId == id);
                    var removeAccepted =  dataModel.PeopleWhoAccepted.FirstOrDefault(y => y.PersonId == id);
                    var removeDeclined =  dataModel.PeopleWhoDeclined.FirstOrDefault(y => y.PersonId == id);

                    //Remove from the invitation list
                    dataModel.PeopleInvited.Remove(removeInitation);

                    //Remove from the accepted list
                    if (removeAccepted != null)
                        dataModel.PeopleWhoAccepted.Remove(removeAccepted);

                    //Remove from the declined list
                    if (removeDeclined != null)
                        dataModel.PeopleWhoDeclined.Remove(removeDeclined);
                });
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
    }
}