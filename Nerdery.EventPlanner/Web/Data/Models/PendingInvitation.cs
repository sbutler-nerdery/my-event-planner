using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Data.Models
{
    /// <summary>
    /// This model is used to store data for people who have been invited to the system but have not created
    /// local accounts yet.
    /// </summary>
    public class PendingInvitation
    {
        public int PendingInvitationId { get; set; }
        /// <summary>
        /// Get or set the id for the person who created the invitation
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// Get or set the email of the person being invited to the system
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Get or set the Facebook id of the person being invited to the system
        /// </summary>
        public string FacebookId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual List<Event> MyInvitations { get; set; }
        public virtual List<Person> MyFriends { get; set; }
    }
}