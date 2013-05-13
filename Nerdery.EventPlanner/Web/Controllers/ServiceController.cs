using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Data;
using Web.Data.Models;
using Web.Extensions;
using WebMatrix.WebData;

namespace Web.Controllers
{
    /// <summary>
    /// This controller is used exclusively for AJAX calls. All of the methods
    /// return JSON objects.
    /// </summary>
    public class ServiceController : Controller
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Person> _personRepository;

        public ServiceController(IRepository<Event> eventRepo, IRepository<Person> personRepo)
        {
            _eventRepository = eventRepo;
            _personRepository = personRepo;
        }

        /// <summary>
        /// Accept an invitation to an event
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="userId">The specified user id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AcceptInviation(int eventId, int userId)
        {
            var response = new Response { Error = false };

            try
            {
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
                var thePerson =
                    _personRepository.GetAll()
                                     .FirstOrDefault(x => x.PersonId == userId);

                thePerson.AmAttending.Add(theEvent);
                thePerson.HaveDeclined.Remove(theEvent);
                _personRepository.SubmitChanges();
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_ACCEPT_INVITATION_FAIL;
            }

            return Json(response);
        }

        /// <summary>
        /// Decline an invitation to an event
        /// </summary>
        /// <param name="eventId">The specified event id</param>
        /// <param name="userId">The specified user id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeclineInviation(int eventId, int userId)
        {
            var response = new Response { Error = false };

            try
            {
                var theEvent = _eventRepository.GetAll().FirstOrDefault(x => x.EventId == eventId);
                var thePerson =
                    _personRepository.GetAll()
                                     .FirstOrDefault(x => x.PersonId == userId);

                thePerson.AmAttending.Remove(theEvent);
                thePerson.HaveDeclined.Add(theEvent);
                _personRepository.SubmitChanges();
            }
            catch (Exception)
            {
                //TODO: log error to database
                response.Error = true;
                response.Message = Constants.SERVICE_ACCEPT_INVITATION_FAIL;
            }

            return Json(response);
        }
    }
}
