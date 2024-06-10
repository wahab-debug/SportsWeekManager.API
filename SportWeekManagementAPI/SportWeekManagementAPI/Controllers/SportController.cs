using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SportWeekManagementAPI.Models;

namespace SportWeekManagementAPI.Controllers
{
    public class SportController : ApiController
    {
        SportsWeekManagementEntities db = new SportsWeekManagementEntities();

        [HttpGet]
        public HttpResponseMessage GetSports()
        {
            try
            {
                var sports = db.Sports.Select(s => new
                { s.id, s.name, s.cric_over, s.cric_ball, s.user_id, s.event_id, s.rule }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, sports);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        //search by id
        public HttpResponseMessage GetSportbyId(int id)
        {
            try
            {
                var sport = db.Sports
               .Where(s => s.id == id)
               .Select(s => new
               { s.id, s.name, s.cric_over, s.cric_ball, s.user_id, s.event_id, s.rule });


                if (sport == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Sport not found");
                }

                return Request.CreateResponse(HttpStatusCode.OK, sport);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]

        public HttpResponseMessage InsertSport([FromBody] Sport sport)
        {
            try
            {
                if (sport == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid data");
                }

                db.Sports.Add(sport);
                db.SaveChanges();

                var response = Request.CreateResponse(HttpStatusCode.Created, sport);
                response.Headers.Location = new Uri(Request.RequestUri + "/" + sport.id);
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteSport(int id)
        {
            try
            {
                var sport = db.Sports.Find(id);
                if (sport == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Sport not found");
                }

                db.Sports.Remove(sport);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Sport Deleted Successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [HttpPost]
        public HttpResponseMessage UpdateSport(Sport sport)
        {
            try
            {
                var existingSport = db.Sports.Find(sport.id);
                if (existingSport == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Sport not found");
                }

                db.Entry(existingSport).CurrentValues.SetValues(sport);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Sport Updated");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}
