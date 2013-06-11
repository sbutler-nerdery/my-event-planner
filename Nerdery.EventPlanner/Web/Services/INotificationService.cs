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
        void SendNotifications(List<EventPlannerNotification> notifications);

        /// <summary>
        /// Get a notification for an updated event.
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="registeredId">An id for a person who is registered in the system</param>
        /// <param name="nonRegisteredId">an id for a person who is not registered in the system</param>
        /// <returns></returns>
        EventPlannerNotification GetNotificationForEventUpdate(int eventId, int registeredId, int nonRegisteredId);
        /// <summary>
        /// Get a notification for an event cancellation.
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="registeredId">An id for a person who is registered in the system</param>
        /// <param name="nonRegisteredId">an id for a person who is not registered in the system</param>
        /// <returns></returns>
        EventPlannerNotification GetNotificationForEventCancelled(int eventId, int registeredId, int nonRegisteredId);

        /// <summary>
        /// Get a notification used to invite a new person to an event.
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="registeredId">An id for a person who is registered in the system</param>
        /// <param name="nonRegisteredId">an id for a person who is not registered in the system</param>
        /// <param name="invitationUrl">The URL the invited person will clik on to accept the inviation</param>
        /// <returns></returns>
        EventPlannerNotification GetNewInvitationNotification(int eventId, int registeredId, int nonRegisteredId, string invitationUrl, string message = "");
        /// <summary>
        /// Get a notification when a person accepts an event invitation.
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="acceptingId">The id of the person accepting the invitation</param>
        /// <returns></returns>
        EventPlannerNotification GetInvitationAcceptedNotification(int eventId, int acceptingId);
        /// <summary>
        /// Get a notification when a person declines an invitation
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="decliningId">The id of the person declining</param>
        /// <returns></returns>
        EventPlannerNotification GetInvitationDeclinedNotification(int eventId, int decliningId);
        /// <summary>
        /// Get a notification when a person is removed from an event.
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="registeredId">An id for a person who is registered in the system</param>
        /// <param name="nonRegisteredId">an id for a person who is not registered in the system</param>
        /// <returns></returns>
        EventPlannerNotification GetPersonRemovedFromEventNotification(int eventId, int registeredId, int nonRegisteredId);
    }
}
