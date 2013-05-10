using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Web.Tests.Services
{
    [TestClass]
    public class NotificationServiceTest
    {
        /// <summary>
        /// This test ensures that if an attendee is removed from an event they are notified.
        /// </summary>
        [TestMethod]
        public void Notification_On_Attendee_Removed()
        {
            //Arrange
            //Get event from repository
            //Act
            //Remove a user from the attendee list
            //Assert            
        }
        /// <summary>
        /// This test ensures that if an event's start date is updated all of the users receive a notification
        /// </summary>
        [TestMethod]
        public void Notification_On_Event_Dates_Updated()
        {

        }
        /// <summary>
        /// This ensures that the correct notification is created for the event coordinator
        /// if an invitee accepts an event invitation.
        /// </summary>
        [TestMethod]
        public void Notification_On_Invitation_Accepted()
        {

        }
        /// <summary>
        /// This ensures that the correct notification is created for the event coordinator
        /// if an invitee declines an event invitation.
        /// </summary>
        [TestMethod]
        public void Notification_On_Invitation_Declined()
        {

        }
        /// <summary>
        /// This test ensures that if an event is deleted from the system, the appropriate notifications are 
        /// generated.
        /// </summary>
        [TestMethod]
        public void Notification_On_Event_Deleted()
        {
            //Arrange

            //Act

            //Assert
        }
    }
}
