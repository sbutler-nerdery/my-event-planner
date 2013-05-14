using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Web.Data.Models;

namespace Web.Data
{
    public class EventPlannerContext : DbContext
    {
        public EventPlannerContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<Game> Games { get; set; }
    }
}