using SportWeekManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SportWeekManagementAPI.Controllers
{
    public class ScheduleController : ApiController
    {
        SportsWeekManagementEntities db = new SportsWeekManagementEntities();

        [HttpGet]
        public HttpResponseMessage AllSchedules()
        {

            try
            {
                var schedule = db.Schedules
                                         .Select(s => new
                                         {
                                             s.id,
                                             s.team1_id,
                                             s.team2_id,
                                             s.match_id,
                                             s.date,
                                             s.time

                                         })
                                         .OrderBy(b => b.id)
                                         .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, schedule);

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage InsertSchedule(Schedule schedule)
        {

            try
            {
                db.Schedules.Add(schedule);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Data Insert at" + schedule.id);


            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }



        [HttpPost]
        public HttpResponseMessage ModifySchedule(Schedule schedule)
        {

            try
            {
                var original = db.Schedules.Find(schedule.id);
                if (original == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Schedule not found");
                }
                db.Entry(original).CurrentValues.SetValues(schedule);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Schedule Modified");



            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteSchedule(int id)
        {

            try
            {
                var original = db.Schedules.Find(id);
                if (original == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Schedule not found");
                }
                db.Entry(original).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Schedule Deleted");



            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage SearchSchedule(string teamName)
        {
            try
            {
                var schedule = db.Schedules
                                 .Join(db.Teams,
                                       s => s.team1_id,
                                       t1 => t1.id,
                                       (s, t1) => new { s, team1 = t1 })
                                 .Join(db.Teams,
                                       st => st.s.team2_id,
                                       t2 => t2.id,
                                       (st, t2) => new { st.s, st.team1, team2 = t2 })
                                 .Where(stt => stt.team1.name == teamName || stt.team2.name == teamName)
                                 .Select(stt => new
                                 {
                                     stt.s.id,
                                     stt.s.team1_id,
                                     team1Name = stt.team1.name,
                                     stt.s.team2_id,
                                     team2Name = stt.team2.name,
                                     stt.s.match_id,
                                     stt.s.date,
                                     stt.s.time
                                 })
                                 .OrderBy(stt => stt.id)
                                 .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, schedule);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}
