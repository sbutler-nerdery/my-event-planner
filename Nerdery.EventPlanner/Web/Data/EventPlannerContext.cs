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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Friends
            modelBuilder.Entity<Person>().HasMany(p => p.MyFriends)
                .WithMany()
                .Map(mc =>
                    {
                        mc.ToTable("PersonFriends");
                        mc.MapLeftKey("PersonId");
                        mc.MapRightKey("FriendId");
                    });

            //Food
            modelBuilder.Entity<Person>().HasMany(e => e.MyFoodItems)
                .WithMany(p => p.OwnerList)
                .Map(mc =>
                {
                    mc.ToTable("PersonFoodItems");
                    mc.MapLeftKey("PersonId");
                    mc.MapRightKey("FoodItemId");
                });

            modelBuilder.Entity<Event>().HasMany(e => e.FoodItems)
                .WithMany(p => p.EventsList)
                .Map(mc =>
                {
                    mc.ToTable("EventFoodItems");
                    mc.MapLeftKey("EventId");
                    mc.MapRightKey("FoodItemId");
                });

            //Games
            modelBuilder.Entity<Person>().HasMany(e => e.MyGames)
                .WithMany(p => p.OwnerList)
                .Map(mc =>
                {
                    mc.ToTable("PersonGames");
                    mc.MapLeftKey("PersonId");
                    mc.MapRightKey("GameId");
                });

            modelBuilder.Entity<Event>().HasMany(e => e.Games)
                .WithMany(p => p.EventsList)
                .Map(mc =>
                {
                    mc.ToTable("EventGames");
                    mc.MapLeftKey("EventId");
                    mc.MapRightKey("GameId");
                });

            //People Invited
            modelBuilder.Entity<Event>().HasMany(e => e.PeopleInvited)
                .WithMany(p => p.MyInvitations)
                .Map(mc =>
                {
                    mc.ToTable("PeopleInvited");
                    mc.MapLeftKey("EventId");
                    mc.MapRightKey("PersonId");
                });

            //People who accepted
            modelBuilder.Entity<Event>().HasMany(e => e.PeopleWhoAccepted)
                .WithMany(p => p.AmAttending)
                .Map(mc =>
                {
                    mc.ToTable("PeopleWhoAccepted");
                    mc.MapLeftKey("EventId");
                    mc.MapRightKey("PersonId");
                });

            //People who declined
            modelBuilder.Entity<Event>().HasMany(e => e.PeopleWhoDeclined)
                .WithMany(p => p.HaveDeclined)
                .Map(mc =>
                {
                    mc.ToTable("PeopleWhoDeclined");
                    mc.MapLeftKey("EventId");
                    mc.MapRightKey("PersonId");
                });

            
        }
    }
}