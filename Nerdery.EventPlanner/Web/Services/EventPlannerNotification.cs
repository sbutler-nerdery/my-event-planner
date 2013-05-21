using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Services
{
    /// <summary>
    /// This class represents a notification sent by the system to either facebook or an email account
    /// </summary>
    public class EventPlannerNotification
    {
        /// <summary>
        /// Get or set the person id who will receive this notification
        /// </summary>
        public int PersonId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// Get or set the email address to send this notification to
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Get or set the facebook id this notification is intended for
        /// </summary>
        public string FacebookId { get; set; }
        /// <summary>
        /// Get or set whether or not to send this notification via Facebook
        /// </summary>
        public bool SendToFacebook { get; set; }
        /// <summary>
        /// Get or set whether or not to send this notification via email
        /// </summary>
        public bool SendToEmail { get; set; }
    }
}