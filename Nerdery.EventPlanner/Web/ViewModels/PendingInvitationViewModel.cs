using System.Collections.Generic;

namespace Web.ViewModels
{
    public class PendingInvitationViewModel
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual List<EditEventViewModel> MyInvitations { get; set; }
        public virtual List<PersonViewModel> MyFriends { get; set; }
    }
}
