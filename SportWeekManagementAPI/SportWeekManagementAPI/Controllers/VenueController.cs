using SportWeekManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SportWeekManagementAPI.Controllers
{
    public class VenueController : ApiController
    {
        SportsManagementDBEntities db = new SportsManagementDBEntities();

        [HttpGet]
        public HttpResponseMessage AllVenues()
        {

            try
            {
                var venue = db.venues
                                         .Select(v => new
                                         {
                                             v.id,
                                             v.name,
                                             v.schedule_id


                                         })
                                         .OrderBy(b => b.id)
                                         .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, venue);

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage InsertVenue(venue venue)
        {

            try
            {
                db.venues.Add(venue);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Data Inserted at " + venue.id);


            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage ModifyVenue(venue venue)
        {

            try
            {
                var original = db.venues.Find(venue.id);
                if (original == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Venue not found");
                }
                db.Entry(original).CurrentValues.SetValues(venue);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Venue Modified");



            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteVenue(int id)
        {

            try
            {
                var original = db.venues.Find(id);
                if (original == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Venue not found");
                }
                db.Entry(original).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Venue Deleted");



            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage SearchVenue(String name)
        {

            try
            {
                var venue = db.venues.Where(b => b.name == name)
                                         .Select(s => new
                                         {
                                             s.id,
                                             s.name,
                                             s.schedule_id

                                         })
                                         .OrderBy(b => b.id)
                                         .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, venue);

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
    