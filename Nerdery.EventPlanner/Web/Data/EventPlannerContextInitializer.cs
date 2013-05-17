using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;
using Web.Data.Models;
using WebMatrix.WebData;

namespace Web.Data
{
    public class EventPlannerContextInitializer : DropCreateDatabaseIfModelChanges<EventPlannerContext>
    {
        protected override void Seed(EventPlannerContext context)
        {
            WebSecurity.InitializeDatabaseConnection(Constants.DB_CONNECTION_STRING,
                                    Constants.DB_USER_TABLE_NAME,
                                    Constants.DB_USER_ID_COLUMN,
                                    Constants.DB_USER_NAME_COLUMN, autoCreateTables: true);

            //Insert a default admin role
            var adminRoleName = Constants.ROLES_ADMIN;
            Roles.CreateRole(adminRoleName);

            //Insert a default admin user
            var userName = "sbutler";

            if (!WebSecurity.UserExists(userName))
            {
                WebSecurity.CreateUserAndAccount(userName, userName,
                    new { 
                        FirstName = "Stan", 
                        LastName = "Butler", 
                        PhoneNumber = "816.123.4567", 
                        Email = "sbutler@nerdery.com",
                        NotifyWithEmail = true,
                        NotifyWithFacebook = false
                    });
            }

            var coordinator = context.People
                .Include("MyFriends")
                .Include("MyFoodItems")
                .Include("MyGames")
                .FirstOrDefault(x => x.UserName == userName);

            //Insert some friends for inviting / un-inviting, etc.
            var ben = new Person
                {
                    UserName = "bbufford",
                    FirstName = "Ben",
                    LastName = "Bufford",
                    PhoneNumber = "719.562.2255",
                    Email = "bbufford@somedomain.com",
                    NotifyWithEmail = true,
                    NotifyWithFacebook = false
                };

            var joe = new Person
            {
                UserName = "jsmith",
                FirstName = "Joe",
                LastName = "Smith",
                PhoneNumber = "816.232.5566",
                Email = "jsmith@somedomain.com",
                NotifyWithEmail = true,
                NotifyWithFacebook = false
            };

            context.People.Add(ben);
            context.People.Add(joe);

            //Add some test food items and games
            var chipsAndDip = new FoodItem {Title = "Chips and Dip", Description = "Tortilla chips and mango salsa. Mmmm. "};
            var settlers = new Game { Title = "Settlers of Catan", Description = "A fun game for 3-4 people."};

            context.Games.Add(settlers);
            context.FoodItems.Add(chipsAndDip);

            //Push changes to the db
            context.SaveChanges();
            
            //Make sure to add friends to the coordinator
            coordinator.MyFriends.Add(ben);
            coordinator.MyFriends.Add(joe);

            //Make sure to add the relationship of the food and games to the coordinator...
            coordinator.MyFoodItems.Add(chipsAndDip);
            coordinator.MyGames.Add(settlers);

            //Push changes to the db
            context.SaveChanges();

            //Insert an event that is already underway
            var defaultEvent = new Event
                {
                    Title = "My First Event",
                    Description = "A seed event",
                    Coordinator = coordinator,
                    Location = "562 Main Street KCMO 64123",
                    PeopleInvited = new List<Person> {joe},
                    FoodItems = new List<FoodItem>{ chipsAndDip },
                    Games = new List<Game> { settlers },
                    StartDate = DateTime.Now.Date.AddHours(17),
                    EndDate = DateTime.Now.Date.AddHours(20)
                };

            context.Events.Add(defaultEvent);

            //Push changes to the db
            context.SaveChanges();

            //Add the user to the admin role... we might use this later for some sort of user management
            if (!Roles.GetRolesForUser(userName).Contains(adminRoleName))
            {
                Roles.AddUserToRole(userName, adminRoleName);
            } 
        }
    }
}