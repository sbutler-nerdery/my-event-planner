using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Services
{
    /// <summary>
    /// This service is primarily in place to make unit testing easier.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Get the user id for the specified user name
        /// </summary>
        /// <param name="userName">A unique user name</param>
        /// <returns></returns>
        int GetCurrentUserId(string userName);
        /// <summary>
        /// Get a list of facebook friends for the specified user name...
        /// </summary>
        /// <param name="userName">A unique user name</param>
        /// <returns></returns>
        List<PersonViewModel> GetFacebookFriends(string userName);
    }
}
