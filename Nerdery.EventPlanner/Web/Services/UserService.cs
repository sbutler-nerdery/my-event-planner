using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Web.Data;
using Web.Data.Models;
using Web.ViewModels;
using WebMatrix.WebData;

namespace Web.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<FacebookAPIValue> _facebookRepository;

        public UserService(IRepositoryFactory factory)
        {
            _facebookRepository = factory.GetRepository<FacebookAPIValue>();
        }

        public int GetCurrentUserId(string userName)
        {
            return WebSecurity.GetUserId(userName);
        }

        public List<PersonViewModel> GetFacebookFriends(string userName)
        {
            var friendsList = new List<PersonViewModel>();

            //Get the API for the current user
            var personId = GetCurrentUserId(userName);
            var facebookData = _facebookRepository.GetAll().FirstOrDefault(x => x.PersonId == personId);

            if (facebookData != null)
            {
                var token = facebookData.AccessToken;
                //Get a list of friends for the currently logged in user
                var getFriendsUrl = string.Format(Constants.GET_FACEBOOK_FRIENDS, token);

                var json = HttpHelper.GetJson(getFriendsUrl);
                var myFriends = JObject.Parse(json);
                var friends = (from friend in myFriends["data"][1]["fql_result_set"]
                               select friend).ToList();

                var tempFriendList = JsonConvert.DeserializeObject<List<FacebookUser>>(JsonConvert.SerializeObject(friends));

                tempFriendList.OrderBy(x => x.Name).ToList()
                    .ForEach(x => friendsList.Add(new PersonViewModel { FacebookId = x.Id, 
                        UserName = x.Name, 
                        ProfilePicUrl = x.SmallPhotoUrl }));
            }

            return friendsList;
        }

        #region Helpers

        /// <summary>
        /// This class is used to spin up web requests and responses to retrieve data other Web APIs.
        /// </summary>
        public class HttpHelper
        {
            /// <summary>
            /// Get a JSON string from the specified Web API url
            /// </summary>
            /// <param name="url">A web API url with the appropriate parameters.</param>
            /// <returns></returns>
            public static string GetJson(string url)
            {
                var jsonString = string.Empty;

                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                try
                {

                    using (var myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse())
                    {
                        // Display the contents of the page to the console.
                        using (var streamResponse = myHttpWebResponse.GetResponseStream())
                        {
                            using (var streamRead = new StreamReader(streamResponse))
                            {
                                jsonString = streamRead.ReadToEnd();
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    //response.Error = true;
                    //response.StatusMessage = ex.Status.ToString();                
                }

                return jsonString;
            }
        }

        /// <summary>
        /// This is a helper class that represents a facebook user.
        /// </summary>
        public class FacebookUser
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("url")]
            public string Link { get; set; }
            [JsonProperty("username")]
            public string UserName { get; set; }
            [JsonProperty("pic")]
            public string SmallPhotoUrl { get; set; }
            [JsonProperty("pic_square")]
            public string LargePhotoUrl { get; set; }
        }

        #endregion
    }
}