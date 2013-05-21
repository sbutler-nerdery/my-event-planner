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
            MyEvents = new List<EventViewModel>();
            AmInvitedToThese = new List<EventViewModel>();
            AmAttending = new List<EventViewModel>();
            HaveDeclined = new List<EventViewModel>();
            MyFoodItems = new List<FoodItemViewModel>();
            MyGames = new List<GameViewModel>();
            MyFriends = new List<PersonViewModel>();
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
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePicUrl { get; set; }
        /// <summary>
        /// Get or set the facebook id for this person
        /// </summary>
        public string FacebookId { get; set; }
        /// <summary>
        /// Get or set events that this user is coordinating
        /// </summary>
        public virtual List<EventViewModel> MyEvents { get; set; }
        /// <summary>
        /// Get or set events that this user is invited to
        /// </summary>
        public virtual List<EventViewModel> AmInvitedToThese { get; set; }
        /// <summary>
        /// Get or set events that this user is attending
        /// </summary>
        public virtual List<EventViewModel> AmAttending { get; set; }
        /// <summary>
        /// Get or set events that this user has declined
        /// </summary>
        public List<EventViewModel> HaveDeclined { get; set; }
        public List<FoodItemViewModel> MyFoodItems { get; set; }
        public List<GameViewModel> MyGames { get; set; }
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
        /// Get or set whether or not this person is selected in a checkbox list
        /// </summary>
        public bool Selected { get; set; }

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
