using SportsWeekManager.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SportsWeekManager.API.Controllers
{
    public class ScheduleController : ApiController
    {
        SportsManagementDBEntities db = new SportsManagementDBEntities();

        
        [HttpGet]
        public HttpResponseMessage getSchedules()
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
        [HttpGet]
        public HttpResponseMessage getSchedule()
        {
            try
            {
                var schedules = db.Schedules.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, schedules);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /*function to check the role of user id and furthur use in schedule create*/
        private bool IsEventManager(int userId)
        {
            var user = db.Users.SingleOrDefault(u => u.id == userId);
            return user != null && user.role == "event manager";
        }
        /*add new schedule just for event manager*/
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

                // Check if the user is an event manager
                if (!IsEventManager(userId))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User is not authorized to create schedules.");
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



    }
}