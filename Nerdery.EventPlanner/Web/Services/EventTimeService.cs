using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Services
{
    public class EventTimeService : IEventTimeService
    {
        public DateTime GetEventEndDate(DateTime startDate, DateTime endDate)
        {
            int endHour = endDate.Hour;
            if (startDate.Hour > endDate.Hour)
                endDate = startDate.AddDays(1).Date.AddHours(endHour);
            
            return endDate;
        }
    }
}