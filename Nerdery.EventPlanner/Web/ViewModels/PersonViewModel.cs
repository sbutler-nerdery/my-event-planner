using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.Data.Models;

namespace Web.ViewModels
{
    public class PersonViewModel
    {
        #region Constructors

        public PersonViewModel()
        {
            MyEvents = new List<EditEventViewModel>();
            AmInvitedToThese = new List<EditEventViewModel>();
            AmAttending = new List<EditEventViewModel>();
            HaveDeclined = new List<EditEventViewModel>();
            MyFoodItems = new List<FoodItemViewModel>();
            MyGames = new List<GameViewModel>();
            MyFriends = new List<PersonViewModel>();
            IsRegistered = true;
        }

        public PersonViewModel(Person model) : this()
        {
            PersonId = model.PersonId;
            UserName = model.UserName;
            FirstName = model.FirstName;
            LastName = model.LastName;
            Email = model.Email;
            PhoneNumber = model.PhoneNumber;
            ProfilePicUrl = model.ProfilePicUrl;
            NotifyWithEmail = model.NotifyWithEmail;
            NotifyWithFacebook = model.NotifyWithFacebook;
        }

        #endregion

        #region Properties

        public int PersonId { get; set; }
        /// <summary>
        /// Get or set the user name for the person
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Get or set the first name of the person
        /// </summary>
        [Required]
        public string FirstName { get; set; }
        /// <summary>
        /// Get or set the last name of the person
        /// </summary>
        [Required]
        public string LastName { get; set; }
        /// <summary>
        /// Get or set the email of the person
        /// </summary>
        [Required]
        public string Email { get; set; }
        /// <summary>
        /// Get or set the phone number of the person
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Get or set the profile picture of the person
        /// </summary>
        public string ProfilePicUrl { get; set; }
        /// <summary>
        /// Get or set the facebook id for this person
        /// </summary>
        public string FacebookId { get; set; }
        /// <summary>
        /// Get or set events that this user is coordinating
        /// </summary>
        public virtual List<EditEventViewModel> MyEvents { get; set; }
        /// <summary>
        /// Get or set events that this user is invited to
        /// </summary>
        public virtual List<EditEventViewModel> AmInvitedToThese { get; set; }
        /// <summary>
        /// Get or set events that this user is attending
        /// </summary>
        public virtual List<EditEventViewModel> AmAttending { get; set; }
        /// <summary>
        /// Get or set events that this user has declined
        /// </summary>
        public List<EditEventViewModel> HaveDeclined { get; set; }
        /// <summary>
        /// Get or set the food items that this person has previously contributed 
        /// </summary>
        public List<FoodItemViewModel> MyFoodItems { get; set; }
        /// <summary>
        /// Get or set the games that this person has previously contributed
        /// </summary>
        public List<GameViewModel> MyGames { get; set; }
        /// <summary>
        /// Get or set the list of friends this user has in the system
        /// </summary>
        public List<PersonViewModel> MyFriends { get; set; }
        /// <summary>
        /// Get or set if the user will receive notifications via facebook.
        /// </summary>
        public bool NotifyWithFacebook { get; set; }
        /// <summary>
        /// Get or set if the user will receive notifications via email.
        /// </summary>
        public bool NotifyWithEmail { get; set; }
        /// <summary>
        /// Get or set whether this person is a registered user in the system
        /// </summary>
        public bool IsRegistered { get; set; }
        /// <summary>
        /// Get or set the message that will be sent to a user when they are invited to an event.
        /// </summary>
        [Display(Name = "Message - (optional)")]
        public string InviteMessage { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Get a Person data model.
        /// </summary>
        /// <returns></returns>
        public Person GetDataModel()
        {
            var dataModel = new Person();
            dataModel.PersonId = PersonId;
            dataModel.UserName = UserName;
            dataModel.FirstName = FirstName;
            dataModel.LastName = LastName;
            dataModel.Email = Email;
            dataModel.PhoneNumber = PhoneNumber;
            dataModel.ProfilePicUrl = ProfilePicUrl;
            dataModel.NotifyWithEmail = NotifyWithEmail;
            dataModel.NotifyWithFacebook = NotifyWithFacebook;
            dataModel.MyFoodItems = new List<FoodItem>();
            dataModel.MyGames = new List<Game>();
            dataModel.AmAttending = new List<Event>();
            dataModel.HaveDeclined = new List<Event>();
            MyFoodItems.ForEach(x => dataModel.MyFoodItems.Add(x.GetDataModel()));
            MyGames.ForEach(x => dataModel.MyGames.Add(x.GetDataModel()));
            AmAttending.ForEach(x => dataModel.AmAttending.Add(x.GetDataModel()));
            HaveDeclined.ForEach(x => dataModel.HaveDeclined.Add(x.GetDataModel()));
            return dataModel;
        }

        #endregion
    }
}
