using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    /// <summary>
    /// An item of food or drink that can be taken to an event
    /// </summary>
    public class FoodItem
    {
        public int FoodItemId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}