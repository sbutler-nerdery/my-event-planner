using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Controllers
{
    /// <summary>
    /// This is a simple class that will be used to pass information
    /// back to the UI when AJAX are performed
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Get or set whether an error occurred
        /// </summary>
        public bool Error { get; set; }
        /// <summary>
        /// Get or set the message to send back to the UI
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Get or set the json data the the response will return
        /// </summary>
        public string Data { get; set; }
    }
}