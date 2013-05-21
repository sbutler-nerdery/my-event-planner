using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.ViewModels;
using WebMatrix.WebData;

namespace Web.Services
{
    public class UserService : IUserService
    {
        public int GetCurrentUserId(string userName)
        {
            return WebSecurity.GetUserId(userName);
        }

        public List<PersonViewModel> GetFacebookFriends(string userName)
        {
            var friendsList = new List<PersonViewModel>();

            //TODO: do a FQL call to get a list of facebook friend by user name
            for (int i = 0; i < 100; i++)
            {
                var friend = new PersonViewModel { ProfilePicUrl = Constants.IMAGE_EMPTY_PROFILE, FirstName = "Friend", LastName = "Number " + i.ToString() };
                friendsList.Add(friend);
            }

            return friendsList;
        }
    }
}