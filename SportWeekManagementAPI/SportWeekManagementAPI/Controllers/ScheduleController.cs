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
                var schedules = from schedule in db.Schedules
                                join match in db.Matches on schedule.match_id equals match.id
                                join team1 in db.Teams on schedule.team1_id equals team1.id
                                join team2 in db.Teams on schedule.team2_id equals team2.id
                                join sport in db.Sports on match.sport_id equals sport.id
                                select new
                                {
                                    Team1Name = team1.name,
                                    Team2Name = team2.name,
                                    MatchId = match.id,
                                    MatchRound = match.round,
                                    SportName = sport.name,
                                    Date = schedule.date,
                                    Time = schedule.time
                                };

                return Request.CreateResponse(HttpStatusCode.OK, schedules.ToList());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private bool IsEventManager(int userId)
        {
            var user = db.Users.SingleOrDefault(u => u.id == userId);
            return user != null && user.role == "event manager";
        }
        [HttpPut]
        public HttpResponseMessage addSchedule(int userId, Schedule schedule)
        {
            try
            {
                // Check if the schedule parameter is null
                if (schedule == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Schedule cannot be null.");
                }

                // Retrieve the user from the database
                var user = db.Users.FirstOrDefault(u => u.id == userId);
                if (user == null || user.role != "event manager")
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User is not authorized to create schedules.");
                }

                // Retrieve the sport managed by the event manager
                var sportManaged = db.Sports.FirstOrDefault(s => s.user_id == userId);
                if (sportManaged == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User does not manage any sport.");
                }

                // Retrieve the teams involved in the schedule
                var team1 = db.Teams.FirstOrDefault(t => t.id == schedule.team1_id);
                var team2 = db.Teams.FirstOrDefault(t => t.id == schedule.team2_id);

                if (team1 == null || team2 == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "One or both teams do not exist.");
                }

                // Check if the teams belong to the sport managed by the event manager
                if (team1.sport_id != sportManaged.id || team2.sport_id != sportManaged.id)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Teams must belong to the sport managed by the event manager.");
                }

                // Check if the match ID is already scheduled
                var existingSchedule = db.Schedules.FirstOrDefault(s => s.match_id == schedule.match_id);
                if (existingSchedule != null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "This match is already scheduled.");
                }

                // Add the schedule
                db.Schedules.Add(schedule);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, schedule.id);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
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
