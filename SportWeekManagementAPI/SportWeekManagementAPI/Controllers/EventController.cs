using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SportWeekManagementAPI.Models;

namespace SportWeekManagementAPI.Controllers
{
    public class EventController : ApiController
    {
        SportsWeekManagementEntities db = new SportsWeekManagementEntities();

        // GET: api/Event
        [HttpGet]

        public HttpResponseMessage GetEvents()
        {
            try
            {
                var events = db.Events
                               .Select(e => new
                               {
                                   e.eventname,
                                   e.year,
                                   e.start_date,
                                   e.end_date
                               })
                               .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, events);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        // GET: api/Event/{id}
        [HttpGet]

        public HttpResponseMessage GetEventById(int id)
        {
            try
            {
                var eventItem = db.Events
                                  .Where(e => e.id == id)
                                  .Select(e => new
                                  {
                                      e.id,
                                      e.eventname,
                                      e.year,
                                      e.user_id,
                                      e.start_date,
                                      e.end_date
                                  })
                                  .FirstOrDefault();

                if (eventItem == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Event not found");
                }

                return Request.CreateResponse(HttpStatusCode.OK, eventItem);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        // POST: api/Event
        [HttpPost]

        public HttpResponseMessage PostEvent([FromBody] Event eventItem)
        {
            try
            {
                if (eventItem == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid data");
                }

                db.Events.Add(eventItem);
                db.SaveChanges();

                var response = Request.CreateResponse(HttpStatusCode.Created, eventItem);
                response.Headers.Location = new Uri(Request.RequestUri + "/" + eventItem.id);
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        // PUT: api/Event/{id}
        [HttpPost]
        public HttpResponseMessage ModifyEvent(Event eventItem)
        {
            try
            {
                var existingEvent = db.Events.Find(eventItem.id);
                if (existingEvent == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Event not found");
                }

                existingEvent.eventname = eventItem.eventname;
                existingEvent.year = eventItem.year;
                existingEvent.start_date = eventItem.start_date;
                existingEvent.end_date = eventItem.end_date;

                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "Event Modified");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        // DELETE: api/Event/{id}
        [HttpDelete]

        public HttpResponseMessage DeleteEvent(int id)
        {
            try
            {
                var eventItem = db.Events.FirstOrDefault(e => e.id == id);
                if (eventItem == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Event not found");
                }

                db.Events.Remove(eventItem);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "Event deleted");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

    }
}
