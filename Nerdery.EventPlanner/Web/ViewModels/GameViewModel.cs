using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ViewModels
{
    /// <summary>
    /// A game that can be taken to an event
    /// </summary>
    public class GameViewModel
    {
        public int GameId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}