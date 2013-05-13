using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Services
{
    /// <summary>
    /// This is a service contract used to send notifications by email or facebook message
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Send an notifications using email and Facebook
        /// </summary>
        /// <param name="notifications">The list of people ids to recieve the message</param>
        void SendNotifications(List<SystemNotification> notifications);
        /// <summary>
        /// Get a list of notifications when an event is updated.
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <returns></returns>
        List<SystemNotification> GetNotificationsForEventUpdate(int eventId);
        /// <summary>
        /// Get a list of notifications when an event is cancelled.
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <returns></returns>
        List<SystemNotification> GetNotificationsForEventCancelled(int eventId);
        /// <summary>
        /// Get a notification when a person accepts an event invitation.
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="acceptingId">The id of the person accepting the invitation</param>
        /// <returns></returns>
        SystemNotification GetInvitationAcceptedNotification(int eventId, int acceptingId);
        /// <summary>
        /// Get a notification when a person declines an invitation
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="decliningId">The id of the person declining</param>
        /// <returns></returns>
        SystemNotification GetInvitationDeclinedNotification(int eventId, int decliningId);
        /// <summary>
        /// Get a notification when a person is removed from an event.
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="removeThisPersonId">The id of the person being removed.</param>
        /// <returns></returns>
        SystemNotification GetPersonRemovedFromEventNotification(int eventId, int removeThisPersonId);
    }
}
