using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Data;
using Web.Models;
using Web.ViewModels;

namespace Web.Controllers
{
    public class EventController : Controller
    {
        private readonly IRepository<Event> _repository;

        public EventController(IRepository<Event> repository)
        {
            _repository = repository;
        }

        //
        // GET: /Event/
        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult Insert(EventViewModel model)
        {
            return View();
        }
    }
}
