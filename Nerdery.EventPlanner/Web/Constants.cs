using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web
{
    /// <summary>
    /// A class that stores all of the constant values used on the site.
    /// </summary>
    public class Constants
    {
        /* Database stuff */
        /// <summary>
        /// The connection string name in web.config
        /// </summary>
        public const string DB_CONNECTION_STRING = "DefaultConnection";
        /// <summary>
        /// The name of the user profile table in the database.
        /// </summary>
        public const string DB_USER_TABLE_NAME = "People";
        /// <summary>
        /// The name of the user id column in the user profile table
        /// </summary>
        public const string DB_USER_ID_COLUMN = "UserId";
        /// <summary>
        /// The name of the user name column in the user profile table
        /// </summary>
        public const string DB_USER_NAME_COLUMN = "UserName";
    }
}