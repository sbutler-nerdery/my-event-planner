using System.Collections.Generic;

namespace Web.Data.Models
{
    /// <summary>
    /// A game that can be taken to an event
    /// </summary>
    public class Game
    {
        public int GameId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual List<Person> OwnerList { get; set; }
        public virtual List<Event> EventsList { get; set; }
    }
}