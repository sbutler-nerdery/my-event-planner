using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace Web.Helpers
{
    /// <summary>
    /// This class manages session objects used by the application
    /// </summary>
    public class SessionHelper
    {
        public static HttpSessionState Session { get; set; }

        /// <summary>
        /// Make sure that the session key value pair is not null
        /// </summary>
        /// <param name="sessionKey"></param>
        private static void EnsureSessionKeyValue(string sessionKey)
        {
            if (Session == null)
                Session = HttpContext.Current.Session;

            if (Session[sessionKey] == null)
                Session[sessionKey] = new List<int>();
        }

        #region Events

        public class Events
        {
            /// <summary>
            /// Reset the session infomation for the specified person id
            /// </summary>
            /// <param name="eventId">An event id</param>
            public static void Reset(int eventId)
            {
                var foodSessionKey = GetFoodSessionKey(eventId);
                var gameSessionKey = GetGameSessionKey(eventId);
                var personSessionKey = GetGuestSessionKey(eventId);
                var messageSessionKey = GetMessageSessionKey(eventId);
                EnsureSessionKeyValue(foodSessionKey);
                EnsureSessionKeyValue(gameSessionKey);
                EnsureSessionKeyValue(personSessionKey);
                EnsureSessionKeyValue(messageSessionKey);
                Session[foodSessionKey] = new List<int>();
                Session[gameSessionKey] = new List<int>();
                Session[personSessionKey] = new List<int>();
                Session[messageSessionKey] = new List<UserMessage>();
            }

            #region Food

            /// <summary>
            /// Get the list of food items that are waiting to be added to the database
            /// </summary>
            /// <param name="eventId"></param>
            /// <returns></returns>
            public static List<int> GetPendingFoodItems(int eventId)
            {
                var sessionKey = GetFoodSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);

                return Session[sessionKey] as List<int>;
            }
            /// <summary>
            /// Add a food item to an event in session
            /// </summary>
            /// <param name="foodItemId">The specified food item id</param>
            /// <param name="eventId">The specified event id</param>
            public static void AddFoodItem(int foodItemId, int eventId)
            {
                var sessionKey = GetFoodSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);
                var foodIds = Session[sessionKey] as List<int>;

                if (!foodIds.Contains(foodItemId))
                    foodIds.Add(foodItemId);
            }
            /// <summary>
            /// Remove a game from an event in session
            /// </summary>
            /// <param name="foodItemId">The specified food item id</param>
            /// <param name="eventId">The specified event id</param>
            public static void RemoveFoodItem(int foodItemId, int eventId)
            {
                var sessionKey = GetFoodSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);

                var foodIds = Session[sessionKey] as List<int>;

                foodIds.Remove(foodItemId);
            }

            #endregion

            #region Games

            /// <summary>
            /// Get the list of games that are waiting to be added to the database
            /// </summary>
            /// <param name="eventId"></param>
            /// <returns></returns>
            public static List<int> GetPendingGames(int eventId)
            {
                var sessionKey = GetGameSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);

                return Session[sessionKey] as List<int>;
            }
            /// <summary>
            /// Add a game to an event in session
            /// </summary>
            /// <param name="gameId">The specified food item id</param>
            /// <param name="eventId">The specified event id</param>
            public static void AddGame(int gameId, int eventId)
            {
                var sessionKey = GetGameSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);
                var gameIds = Session[sessionKey] as List<int>;

                if (!gameIds.Contains(gameId))
                    gameIds.Add(gameId);
            }
            /// <summary>
            /// Remove a game from an event in session
            /// </summary>
            /// <param name="gameId">The specified food item id</param>
            /// <param name="eventId">The specified event id</param>
            public static void RemoveGame(int gameId, int eventId)
            {
                var sessionKey = GetGameSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);

                var gameIds = Session[sessionKey] as List<int>;

                gameIds.Remove(gameId);
            }

            #endregion

            #region Guest list
            /// <summary>
            /// Get the list of food items that are waiting to be added to the database
            /// </summary>
            /// <param name="eventId"></param>
            /// <returns></returns>
            public static List<int> GetGuestList(int eventId)
            {
                var sessionKey = GetGuestSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);

                return Session[sessionKey] as List<int>;
            }
            /// <summary>
            /// Add a person id as a guest of an event 
            /// </summary>
            /// <param name="personId">The specified person id</param>
            /// <param name="eventId">The specified event id</param>
            public static void AddGuest(int personId, int eventId)
            {
                var sessionKey = GetGuestSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);
                var personIds = Session[sessionKey] as List<int>;

                if (!personIds.Contains(personId))
                    personIds.Add(personId);
            }
            /// <summary>
            /// Remove a person id as an event guest 
            /// </summary>
            /// <param name="personId">The specified person id</param>
            /// <param name="eventId">The specified event id</param>
            public static void RemoveGuest(int personId, int eventId)
            {
                var sessionKey = GetGuestSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);

                var personIds = Session[sessionKey] as List<int>;

                personIds.Remove(personId);
            }

            #endregion

            #region Messages
            /// <summary>
            /// Get the list of personalized messages that will be send to guests for the specified
            /// event id.
            /// </summary>
            /// <param name="eventId"></param>
            /// <returns></returns>
            public static List<UserMessage> GetMessageList(int eventId)
            {
                var sessionKey = GetMessageSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);
                var messageList = Session[sessionKey] as List<UserMessage>;

                return messageList;
            }
            /// <summary>
            /// Add a message to an event for the specified guest id
            /// </summary>
            /// <param name="eventId"></param>
            /// <param name="guestId"></param>
            /// <param name="text"></param>
            public static void AddOrUpdateMessage(int eventId, int guestId, string text)
            {
                var sessionKey = GetMessageSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);
                var messageList = Session[sessionKey] as List<UserMessage>;
                var message = messageList.FirstOrDefault(x => x.GuestId.Equals(guestId));

                if (message == null)
                    messageList.Add(new UserMessage { GuestId = guestId, Message = text });
                else
                    message.Message = text;

                Session[sessionKey] = messageList;
            }
            /// <summary>
            /// Remove a message from an event for the specified guest id
            /// </summary>
            /// <param name="eventId"></param>
            /// <param name="guestId"></param>
            public static void RemoveMessage(int eventId, int guestId)
            {
                var sessionKey = GetMessageSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);
                var messageList = Session[sessionKey] as List<UserMessage>;
                var removeMe = messageList.FirstOrDefault(x => x.GuestId.Equals(guestId));

                if (removeMe != null)
                    messageList.Remove(removeMe);

                Session[sessionKey] = messageList;
            }

            #endregion

            #region Session Keys

            /// <summary>
            /// Get the food session key for the specified event id
            /// </summary>
            /// <param name="eventId"></param>
            /// <returns></returns>
            private static string GetFoodSessionKey(int eventId)
            {
                return "food-for-event-" + eventId.ToString();
            }
            /// <summary>
            /// Get the game session key for the specified event id
            /// </summary>
            /// <param name="eventId"></param>
            /// <returns></returns>
            private static string GetGameSessionKey(int eventId)
            {
                return "game-for-event-" + eventId.ToString();
            }
            /// <summary>
            /// Get the guest list session key for the specified event id
            /// </summary>
            /// <param name="eventId"></param>
            /// <returns></returns>
            private static string GetGuestSessionKey(int eventId)
            {
                return "invited-to-event-" + eventId.ToString();
            }
            /// <summary>
            /// Get the custom invitation message list session key for the specified event id
            /// </summary>
            /// <param name="eventId"></param>
            /// <returns></returns>
            private static string GetMessageSessionKey(int eventId)
            {
                return "custom-messages-for-event-" + eventId.ToString();
            }

            #endregion
        }

        #endregion

        #region Person

        public class Person
        {
            /// <summary>
            /// Reset the session infomation for the specified person id
            /// </summary>
            /// <param name="personId">A user id</param>
            public static void Reset(int personId)
            {
                var foodSessionKey = GetFoodSessionKey(personId);
                var gameSessionKey = GetGameSessionKey(personId);
                EnsureSessionKeyValue(foodSessionKey);
                EnsureSessionKeyValue(gameSessionKey);
                Session[foodSessionKey] = new List<int>();
                Session[gameSessionKey] = new List<int>();
            }

            #region Food

            /// <summary>
            /// Get the list of food items that are waiting to be added to the database
            /// </summary>
            /// <param name="personId"></param>
            /// <returns></returns>
            public static List<int> GetPendingFoodItems(int personId)
            {
                var sessionKey = GetFoodSessionKey(personId);
                EnsureSessionKeyValue(sessionKey);

                return Session[sessionKey] as List<int>;
            }
            /// <summary>
            /// Add a food item to a person in session
            /// </summary>
            /// <param name="foodItemId">The specified food item id</param>
            /// <param name="personId">The specified event id</param>
            public static void AddFoodItem(int foodItemId, int personId)
            {
                var sessionKey = GetFoodSessionKey(personId);
                EnsureSessionKeyValue(sessionKey);
                var foodIds = Session[sessionKey] as List<int>;

                if (!foodIds.Contains(foodItemId))
                    foodIds.Add(foodItemId);
            }
            /// <summary>
            /// Remove a game from an person in session
            /// </summary>
            /// <param name="foodItemId">The specified food item id</param>
            /// <param name="personId">The specified event id</param>
            public static void RemoveFoodItem(int foodItemId, int personId)
            {
                var sessionKey = GetFoodSessionKey(personId);
                EnsureSessionKeyValue(sessionKey);

                var foodIds = Session[sessionKey] as List<int>;

                foodIds.Remove(foodItemId);
            }

            #endregion

            #region Games

            /// <summary>
            /// Get the list of games that are waiting to be added to the database
            /// </summary>
            /// <param name="eventId"></param>
            /// <returns></returns>
            public static List<int> GetPendingGames(int eventId)
            {
                var sessionKey = GetGameSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);

                return Session[sessionKey] as List<int>;
            }
            /// <summary>
            /// Add a game to a person in session
            /// </summary>
            /// <param name="gameId">The specified food item id</param>
            /// <param name="personId">The specified event id</param>
            public static void AddGame(int gameId, int personId)
            {
                var sessionKey = GetGameSessionKey(personId);
                EnsureSessionKeyValue(sessionKey);
                var gameIds = Session[sessionKey] as List<int>;

                if (!gameIds.Contains(gameId))
                    gameIds.Add(gameId);
            }
            /// <summary>
            /// Remove a game from a person in session
            /// </summary>
            /// <param name="gameId">The specified food item id</param>
            /// <param name="personId">The specified event id</param>
            public static void RemoveGame(int gameId, int personId)
            {
                var sessionKey = GetGameSessionKey(personId);
                EnsureSessionKeyValue(sessionKey);

                var gameIds = Session[sessionKey] as List<int>;

                gameIds.Remove(gameId);
            }

            #endregion

            #region Session Keys

            /// <summary>
            /// Get the food session key for the specified person id
            /// </summary>
            /// <param name="personId"></param>
            /// <returns></returns>
            private static string GetFoodSessionKey(int personId)
            {
                return "food-for-person-" + personId.ToString();
            }
            /// <summary>
            /// Get the game session key for the specified person id
            /// </summary>
            /// <param name="personId"></param>
            /// <returns></returns>
            private static string GetGameSessionKey(int personId)
            {
                return "game-for-person-" + personId.ToString();
            }

            #endregion 
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Represents a guest id and a message to be sent to it.
        /// </summary>
        public class UserMessage
        {
            /// <summary>
            /// Get or set the guest id
            /// </summary>
            public int GuestId { get; set; }
            /// <summary>
            /// Get or set the message text
            /// </summary>
            public string Message { get; set; }
        }

        #endregion
    }
}