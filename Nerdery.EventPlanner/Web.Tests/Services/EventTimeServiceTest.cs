using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Services;

namespace Web.Tests.Services
{
    [TestClass]
    public class EventTimeServiceTest
    {
        /// <summary>
        /// This test ensures that the start date for an event is parsed together correctly from the parameter
        /// values that would be supplied from a view model
        /// </summary>
        [TestMethod]
        public void Create_Event_Parse_Start_Date()
        {
            var service = new EventService();
            var startDateValue = DateTime.Now;
            var startTimeValue = DateTime.Now.Date.AddHours(4).AddMinutes(30); //4:30 AM today
            var compiledStartDate = service.GetEventStartDate(startDateValue, startTimeValue);
            Assert.AreEqual(compiledStartDate, startTimeValue);
        }
        /// <summary>
        /// This test ensures that if the end time in the view model is past midnight the end date of the data model
        /// is adjusted to the next day properly.
        /// </summary>
        [TestMethod]
        public void Create_Event_Handle_Past_Midnight()
        {
            //Arrange
            var service = new EventService();
            var startDate = DateTime.Now.Date.AddHours(19); //7PM
            var endDate = DateTime.Now.Date.AddHours(2); //This SHOULD be 2AM the next day...

            //Act
            var modifiedEndDate = service.GetEventEndDate(startDate, endDate);

            //Assert that the new date is 2AM the next day
            Assert.AreEqual(DateTime.Now.Date.AddHours(26), modifiedEndDate);
        }
    }
}
