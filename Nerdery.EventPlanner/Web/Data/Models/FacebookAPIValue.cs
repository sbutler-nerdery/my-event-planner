using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Data.Models
{
    public class FacebookAPIValue
    {
        /// <summary>
        /// Get or set the unique key
        /// </summary>
        public int FacebookAPIValueId { get; set; }
        /// <summary>
        /// Get or set the access token used by the current facebook session
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// Get or set the person id that an access token is tied to
        /// </summary>
        public int PersonId { get; set; }
    }
}