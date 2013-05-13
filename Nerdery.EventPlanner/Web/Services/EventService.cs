﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Data.Models;
using Web.ViewModels;

namespace Web.Services
{
    public class EventService : IEventService
    {
        public DateTime GetEventStartDate(DateTime startDate, DateTime startTime)
        {
            int hours = startTime.Hour;
            int minutes = startTime.Minute;

            return startDate.Date.AddHours(hours).AddMinutes(minutes);
        }
        public DateTime GetEventEndDate(DateTime startDate, DateTime endDate)
        {
            int endHour = endDate.Hour;
            if (startDate.Hour > endDate.Hour)
                endDate = startDate.AddDays(1).Date.AddHours(endHour);
            
            return endDate;
        }

        public void AppendNewFoodItems(Event dataModel, EventViewModel viewModel)
        {
            var dataFoodIds = dataModel.FoodItems.Select(x => x.FoodItemId).ToArray(); //Items in the database
            var newFoodItems = viewModel.FoodItems.Where(x => !dataFoodIds.Contains(x.FoodItemId)).ToList();

            //Add new items
            newFoodItems.ForEach(x => dataModel.FoodItems.Add(x.GetDataModel()));
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
            newGameItems.ForEach(x => dataModel.Games.Add(x.GetDataModel()));
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
                var newPeople = viewModel.PeopleInvited.Where(x => !dataPeopleIds.Contains(x.PersonId)).ToList();

                //Add new people
                newPeople.ForEach(x => dataModel.PeopleInvited.Add(new Person{ PersonId = x.PersonId, FirstName = x.FirstName, LastName = x.LastName }));

        }

        public void UninvitePeople(Event dataModel, EventViewModel viewModel)
        {
                var modelPeopleIds = viewModel.PeopleInvited.Select(x => x.PersonId).ToArray(); //Items in local view model
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
    }
}