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
        /// This test ensures that if the end time in the view model is past midnight the end date of the data model
        /// is adjusted to the next day properly.
        /// </summary>
        [TestMethod]
        public void Create_Event_Handle_Past_Midnight()
        {
            //Arrange
            var service = new EventTimeService();
            var startDate = DateTime.Now.Date.AddHours(19); //7PM
            var endDate = DateTime.Now.Date.AddHours(2); //This SHOULD be 2AM the next day...

            //Act
            var modifiedEndDate = service.GetEventEndDate(startDate, endDate);

            //Assert that the new date is 2AM the next day
            Assert.AreEqual(DateTime.Now.Date.AddHours(26), modifiedEndDate);
        }
    }
}
