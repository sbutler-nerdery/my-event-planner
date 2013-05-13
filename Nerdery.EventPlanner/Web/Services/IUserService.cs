using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Services
{
    /// <summary>
    /// This service is primarily in place to make unit testing easier.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Get the current user's user id from the database...
        /// </summary>
        /// <returns></returns>
        int GetCurrentUserId(string userName);
    }
}
