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
        #region Database stuff

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
        public const string DB_USER_ID_COLUMN = "PersonId";
        /// <summary>
        /// The name of the user name column in the user profile table
        /// </summary>
        public const string DB_USER_NAME_COLUMN = "UserName";

        #endregion

        #region Roles

        public const string ROLES_ADMIN = "Admin";

        #endregion 

        #region Account controller messages

        public const string ACCOUNT_PASSWORD_CHANGE_SUCCESS = "Your password has been changed.";
        public const string ACCOUNT_PASSWORD_SET = "Your password has been set.";
        public const string ACCOUNT_EXTERNAL_LOGIN_REMOVED = "The external login was removed.";
        public const string ACCOUNT_UPDATE_USER_INFO_SUCCESS = "User info updated successfully.";
        public const string ACCOUNT_UPDATE_USER_INFO_FAIL = "There was an error while trying to update user info.";
        #endregion

        #region Base controller messages

        public const string BASE_SAVE_SUCCESS = "Your changes were saved successfully!";
        public const string BASE_SAVE_FAIL = "An error occurrred while trying to save changes.";
        public const string BASE_DELETE_SUCCESS = "Delete succedssful.";
        public const string BASE_DELETE_FAIL = "An error occurrred while trying to delete.";
        public const string BASE_BUILD_VIEW_FAIL = "An error occurrred while retrieving your data.";
        public const string BASE_ACCEPT_INVITATION_FAIL = "An error occurred. Unable to accept invitation.";
        public const string BASE_ACCEPT_INVITATION_SUCCESS = "You have accepted the invitation!";
        public const string BASE_DECLINE_INVITATION_FAIL = "An error occurred. Unable to decline invitation.";
        public const string BASE_DECLINE_INVITATION_SUCCESS = "You have declined the invitation!";
        public const string BASE_UPDATE_INVITATION_FAIL = "An error occurred. Unable to update invitation.";
        public const string BASE_UPDATE_INVITATION_SUCCESS = "Event updated successfully!";

        #endregion 

        #region Service controller message

        public const string SERVICE_ADD_FOOD_ITEM_FAIL = "An error occurred. Unable to add the specified food item.";
        public const string SERVICE_REMOVE_FOOD_ITEM_FAIL = "An error occurred. Unable to remove the specified food item.";
        public const string SERVICE_GET_FOOD_ITEM_FAIL = "An error occurred. Unable to retieve the specified food item.";
        public const string SERVICE_UPDATE_FOOD_ITEM_FAIL = "An error occurred. Unable to update the food item.";

        public const string SERVICE_ADD_GAME_FAIL = "An error occurred. Unable to add the specified game.";
        public const string SERVICE_REMOVE_GAME_FAIL = "An error occurred. Unable to remove the specified game.";
        public const string SERVICE_GET_GAME_FAIL = "An error occurred. Unable to retieve the specified game.";
        public const string SERVICE_UPDATE_GAME_FAIL = "An error occurred. Unable to update the game.";

        public const string SERVICE_ADD_GUEST_FAIL = "An error occurred. Unable to add the guest.";
        public const string SERVICE_REMOVE_GUEST_FAIL = "An error occurred. Unable to remove the guest.";
        public const string SERVICE_GET_GUEST_FAIL = "An error occurred. Unable to retieve the guest.";
        public const string SERVICE_UPDATE_GUEST_FAIL = "An error occurred. Unable to update the guest.";


        #endregion

        #region Notification message templates
        /// <summary>
        /// This is the title used for notifications that are sent to updates people when an event changes.
        /// </summary>
        public const string MESSAGE_UPDATE_TITLE = "Event Update Notification";
        /// <summary>
        /// The message template used to notify users if an event has been updated.
        /// {0} = event name
        /// {1} = coordinator name
        /// {2} = new event date
        /// {3} = new event time
        /// </summary>
        public const string MESSAGE_UPDATE_TEMPLATE =
            "The event title '{0}' hosted by {1} has will now start on {2} at {3}.";
        /// <summary>
        /// The title used for notifications that are sent to updates people when an event is cancelled.
        /// </summary>
        public const string MESSAGE_CANCELLED_TITLE = "An event has been cancelled.";
        /// <summary>
        /// The message template used to notify users if an event has been cancelled.
        /// {0} = event name
        /// {1} = coordinator name
        /// </summary>
        public const string MESSAGE_CANCELLED_TEMPLATE =
            "The event title '{0}' hosted by {1} has been cancelled.";
        /// <summary>
        /// The title used to invite a user to an event
        /// </summary>
        public const string MESSAGE_NEW_TITLE = "You have been invited to an event!";
        /// <summary>
        /// The message template used to invite a user to an event.
        /// {0} = Event host name
        /// {1} = Event name
        /// {2} = Event date
        /// {3} = Event time
        /// {4} = The event invitation URL
        /// </summary>
        public const string MESSAGE_NEW_TEMPLATE =
            "{0} has invited you to the event '{1}' starting on {2} at {3}! <a href='{4}'>Click here</a> to accept.";
        /// <summary>
        /// The title used when a user accepts an invitation
        /// </summary>
        public const string MESSAGE_ACCEPT_TITLE = "Good news!";
        /// <summary>
        /// The message template used to notify users if an event has been cancelled.
        /// {0} = Attendee name
        /// {1} = Event name
        /// {2} = Event date
        /// {3} = event time
        /// {4} = food items
        /// {5} = games
        /// </summary>
        public const string MESSAGE_ACCEPT_TEMPLATE =
            "{0} has accepted your generous invitation for the event '{1}' starting on {2} at {3}.{4}{5}";
        /// <summary>
        /// The title used when a user accepts an invitation
        /// </summary>
        public const string MESSAGE_DECLINE_TITLE = "Sorry to tell you this!";
        /// <summary>
        /// The message template used to notify users if an event has been cancelled.
        /// {0} = Declinee first name
        /// {1} = Event name
        /// {2} = Event date
        /// {3} = event time
        /// </summary>
        public const string MESSAGE_DECLINE_TEMPLATE =
            "{0} won't be coming to the event '{1}' starting on {2} at {3}.";
        /// <summary>
        /// The title used when a user accepts an invitation
        /// </summary>
        public const string MESSAGE_REMOVE_TITLE = "You have been un-invited";
        /// <summary>
        /// The message template used to notify users if an event has been cancelled.
        /// {0} = Removee name
        /// {1} = Event name
        /// {2} = Event date
        /// {3} = event time
        /// </summary>
        public const string MESSAGE_REMOVE_TEMPLATE =
            "{0} has removed you from the guest list for the event '{1}' starting on {2} at {3}.";

        #endregion

        #region Facebook Queries

        public const string GET_FACEBOOK_FRIENDS = "https://graph.facebook.com/fql?q={{\"friendsIds\":\"SELECT+uid2+FROM+friend+WHERE+uid1=me()\",\"friends\":\"SELECT+id,+name,+url,+pic,+pic_big+FROM+profile+WHERE+id+IN+(SELECT+uid2+FROM+%23friendsIds)\"}}&access_token={0}";

        #endregion 

        #region Images

        public const string IMAGE_EMPTY_PROFILE =
            @"data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBwcKCgwMCAgHBwcHEAoHCAgHBw8ICQgKFBEWFhQRExMYHCggGBoxGxMTITEhJSkrLi4uFx8zODMsNygtLisBCgoKDA0MDgwMDiwZHxkrKywrKysrLCsrKysrLCsrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrK//AABEIANkAyAMBIgACEQEDEQH/xAAZAAEBAQEBAQAAAAAAAAAAAAAAAQQDAgf/xAAoEAEBAAECBQMEAwEAAAAAAAAAAQIEMQMRMnGBIUFREiJhkUJisVL/xAAWAQEBAQAAAAAAAAAAAAAAAAAAAQL/xAAUEQEAAAAAAAAAAAAAAAAAAAAA/9oADAMBAAIRAxEAPwD7GCMqKgCgAAAAAlCgAAKgAKigIqAqKAigBAgoAiBQAUAAABAAAtk3snegCfXh/wBT9ksu1l7UFAAVFAAAAAAAgQUAEARQAABAB5z4mOO/iLlZJzvsxZZXK86D3nxs8v6z4jmIKKAOmHGzm95z8tPD4mOW36YlxyuN5zcRuV5xss5zavQAAAAAAECCgAgigAACBQHHVX0k+WVo1W88uACKgqgAAA06W+lnxysd2bS71pEAABABUUCBBQRRBFRQAQFEoDPqt55Z3fVbzy4gIUFFAAAHbS71qZdL1VqEBKAoAAAECCgiiCCgIoAlFQGbVbzs4tGqx2vx6M4IKCgAAAO2l6r2ambSze+GgQVFAAAAAgQUEUQRUUAAEoUBM8fqlnyw2cryu8b2fU4fynkHBFQVRFAJ/o76bDn919tgduHj9Mk+N+72AgigAAAAECCgiiCKAAgAAAmUllnyrxxcvpx/O0BjABFAUbsJJJIwtnCzmWM+Z6UR7VAFAAAAEAWBBQAQAQFEtk3sc8uNw5787+AdDnJv6M2Woy/jPp/Llllld7aDTnx8Jt91Z888srzrwoAAoAAuGeWN5z9IgNeHHxvV9tdecu3r2YFxyym1sEblZcdRlN5zdcePw778u4Ooksu15qAABAgoAII4cTUe2Hmmp4nL7ZfX3ZwW5ZXe290AUAARQEUAAAQUARQAAFmWU2tjtwuP7ZftwBG9WfTcS9N8NAECCglV5zvLG+UGLO87b8vIAoAoAAAAAAAAAACAoigAAY3lZfhvYG7C88YI9QIKDnx+iujlqOi+EGQEFUAAAAAAAAAAABFAEUAAAbODfsjE2abonkR1gQUHHVdPmOzhqumd0GVQFAAAAAAAAAAAAAAAARQAatN0+WVp0vTe4jvAgolcNVtO7RWfVbTygzgCgAAAAAAAAAAAIqKAAAAA0aTbLwztGk2y8CNECCj/2Q==";

        #endregion

        #region String delimiters

        public const char EVENT_INVITE_DELIMITER = '╫';

        #endregion
    }
}