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
        #region Fields

        private readonly IRepository<Event> _repository;

        #endregion

        #region Constructors

        public EventController(IRepository<Event> repository)
        {
            _repository = repository;
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// Create a new event
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(EventViewModel model)
        {
            return View();
        }

        /// <summary>
        /// Edit an existing event
        /// </summary>
        /// <param name="id">The specified event id</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(EventViewModel model)
        {
            return View();
        }

        /// <summary>
        /// Delete an event, which is the same as cancelling an event.
        /// </summary>
        /// <param name="id">The specified event id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
