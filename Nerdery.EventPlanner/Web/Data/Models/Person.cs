using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Data.Models
{
    [Table(Constants.DB_USER_TABLE_NAME)]
    public class Person
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PersonId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePicUrl { get; set; }
        /// <summary>
        /// Get or set events that this user is coordinating
        /// </summary>
        public virtual List<Event> MyEvents { get; set; }
        /// <summary>
        /// Get or set events that this user is invited to
        /// </summary>
        public virtual List<Event> MyInvitations { get; set; }
        /// <summary>
        /// Get or set events that this user is attending
        /// </summary>
        public virtual List<Event> AmAttending { get; set; }
        /// <summary>
        /// Get or set events that this user has declined
        /// </summary>
        public virtual List<Event> HaveDeclined { get; set; }
        public virtual List<FoodItem> MyFoodItems { get; set; }
        public virtual List<Game> MyGames { get; set; }
        public virtual List<Person> MyFriends { get; set; }
        /// <summary>
        /// Get or set a list of friends that have been invited to an event, but have not created user accounts yet.
        /// </summary>
        public virtual List<PendingInvitation> MyPendingFriends { get; set; }
        /// <summary>
        /// Get or set if the user will receive notifications via facebook.
        /// </summary>
        public bool NotifyWithFacebook { get; set; }
        /// <summary>
        /// Get or set if the user will receive notifications via email.
        /// </summary>
        public bool NotifyWithEmail { get; set; }
    }
}
