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

        /* Notification message templates */
        /// <summary>
        /// This is the title used for notifications that are sent to updates people when an event changes.
        /// </summary>
        public const string MESSAGE_UPDATE_TITLE = "Event Update Notification";
        /// <summary>
        /// The message template used to notify users if an event has been updated.
        /// {0} = event name
        /// {1} = coordinator first name
        /// {2} = coordinator last name
        /// {3} = new event date
        /// {4} = new event time
        /// </summary>
        public const string MESSAGE_UPDATE_TEMPLATE =
            "The event title '{0}' hosted by {1} {2} has will now start on {3} at {4}.";
        /// <summary>
        /// The title used for notifications that are sent to updates people when an event is cancelled.
        /// </summary>
        public const string MESSAGE_CANCELLED_TITLE = "An event has been cancelled.";
        /// <summary>
        /// The message template used to notify users if an event has been cancelled.
        /// {0} = event name
        /// {1} = coordinator first name
        /// {2} = coordinator last name
        /// </summary>
        public const string MESSAGE_CANCELLED_TEMPLATE =
            "The event title '{0}' hosted by {1} {2} has been cancelled.";
        /// <summary>
        /// The title used when a user accepts an invitation
        /// </summary>
        public const string MESSAGE_ACCEPT_TITLE = "Good news!";
        /// <summary>
        /// The message template used to notify users if an event has been cancelled.
        /// {0} = Attendee first name
        /// {1} = Attendee last name
        /// {2} = Event name
        /// {3} = Event date
        /// {4} = event time
        /// </summary>
        public const string MESSAGE_ACCEPT_TEMPLATE =
            "{0} {1} has accepted your generous invitation for the event '{2}' starting on {3} at {4}.";
        /// <summary>
        /// The title used when a user accepts an invitation
        /// </summary>
        public const string MESSAGE_DECLINE_TITLE = "Sorry to tell you this!";
        /// <summary>
        /// The message template used to notify users if an event has been cancelled.
        /// {0} = Declinee first name
        /// {1} = Declinee last name
        /// {2} = Event name
        /// {3} = Event date
        /// {4} = event time
        /// </summary>
        public const string MESSAGE_DECLINE_TEMPLATE =
            "{0} {1} won't be coming to the event '{2}' starting on {3} at {4}.";
        /// <summary>
        /// The title used when a user accepts an invitation
        /// </summary>
        public const string MESSAGE_REMOVE_TITLE = "You have been un-invited";
        /// <summary>
        /// The message template used to notify users if an event has been cancelled.
        /// {0} = Removee first name
        /// {1} = Removee last name
        /// {2} = Event name
        /// {3} = Event date
        /// {4} = event time
        /// </summary>
        public const string MESSAGE_REMOVE_TEMPLATE =
            "{0} {1} has removed you from the guest list for the event '{2}' starting on {3} at {4}.";
    }
}