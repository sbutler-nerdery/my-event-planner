using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Services
{
    /// <summary>
    /// This is a service contract used for correctly updating the start / end date time values for events
    /// </summary>
    public interface IEventTimeService
    {
        /// <summary>
        /// Get the correct end date value for the start and end date pair provided.
        /// </summary>
        /// <param name="startDate">The specified start date</param>
        /// <param name="endDate">The specified end date to be modified.</param>
        /// <example>start date: 4/4/2012 4:15 PM, end date: 4/4/2012 1:00 AM will return an end date value of 4/5/2012 1:00 AM</example>
        DateTime GetEventEndDate(DateTime startDate, DateTime endDate);
    }
}
