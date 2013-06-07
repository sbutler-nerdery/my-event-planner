using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

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

            if (Events.EventIds == null)
                Events.EventIds = new List<int>();

            if (Person.PersonIds == null)
                Person.PersonIds = new List<int>();
        }

        #region Events

        public class Events
        {
            public static List<int> EventIds { get; set; }

            /// <summary>
            /// Reset the session infomation for the specified person id
            /// </summary>
            /// <param name="eventId">An event id</param>
            public static void Reset(int eventId)
            {
                var foodSessionKey = GetFoodSessionKey(eventId);
                var gameSessionKey = GetGameSessionKey(eventId);
                var personSessionKey = GetInviteeSessionKey(eventId);
                EnsureSessionKeyValue(foodSessionKey);
                EnsureSessionKeyValue(gameSessionKey);
                EnsureSessionKeyValue(personSessionKey);
                Session[foodSessionKey] = new List<int>();
                Session[gameSessionKey] = new List<int>();
                Session[personSessionKey] = new List<int>();
            }
            /// <summary>
            /// Get the list of food items that are waiting to be added to the database
            /// </summary>
            /// <param name="eventId"></param>
            /// <returns></returns>
            public static List<int> GetPendingFoodItems(int eventId)
            {
                var sessionKey = GetFoodSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);

                if (!EventIds.Contains(eventId))
                    EventIds.Add(eventId);

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
            /// <summary>
            /// Get the list of food items that are waiting to be added to the database
            /// </summary>
            /// <param name="eventId"></param>
            /// <returns></returns>
            public static List<int> GetPendingInvites(int eventId)
            {
                var sessionKey = GetFoodSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);

                if (!EventIds.Contains(eventId))
                    EventIds.Add(eventId);

                return Session[sessionKey] as List<int>;
            }
            /// <summary>
            /// Add a person id as a guest of an event 
            /// </summary>
            /// <param name="personId">The specified person id</param>
            /// <param name="eventId">The specified event id</param>
            public static void AddGuest(int personId, int eventId)
            {
                var sessionKey = GetFoodSessionKey(eventId);
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
                var sessionKey = GetFoodSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);

                var personIds = Session[sessionKey] as List<int>;

                personIds.Remove(personId);
            }
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
            /// Get the invitee session key for the specified event id
            /// </summary>
            /// <param name="eventId"></param>
            /// <returns></returns>
            private static string GetInviteeSessionKey(int eventId)
            {
                return "invited-to-event-" + eventId.ToString();
            }
        }

        #endregion

        #region Person

        public class Person
        {
            public static List<int> PersonIds { get; set; }

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

            /// <summary>
            /// Get the list of food items that are waiting to be added to the database
            /// </summary>
            /// <param name="personId"></param>
            /// <returns></returns>
            public static List<int> GetPendingFoodItems(int personId)
            {
                var sessionKey = GetFoodSessionKey(personId);
                EnsureSessionKeyValue(sessionKey);

                if (!PersonIds.Contains(personId))
                    PersonIds.Add(personId);

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
            /// <summary>
            /// Get the list of games that are waiting to be added to the database
            /// </summary>
            /// <param name="eventId"></param>
            /// <returns></returns>
            public static List<int> GetPendingGames(int eventId)
            {
                var sessionKey = GetGameSessionKey(eventId);
                EnsureSessionKeyValue(sessionKey);

                if (!PersonIds.Contains(eventId))
                    PersonIds.Add(eventId);

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
        }

        #endregion
    }
}