using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace Web.Services
{
    public class UserService : IUserService
    {
        public int GetCurrentUserId(string userName)
        {
            return WebSecurity.GetUserId(userName);
        }
    }
}