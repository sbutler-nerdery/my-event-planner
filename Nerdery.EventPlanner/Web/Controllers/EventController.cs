using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Data;
using Web.Data.Models;
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

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(EventViewModel model)
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(EventViewModel model)
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteEvent(int id)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
