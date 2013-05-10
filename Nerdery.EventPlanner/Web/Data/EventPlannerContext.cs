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

        public DbSet<Person> UserProfiles { get; set; }
    }
}