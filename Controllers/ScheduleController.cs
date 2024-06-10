using SportsWeekManager.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using Match = SportsWeekManager.API.Models.Match;

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

        /*add schedule with userid and schedule parameter*/
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

                // Create a new match
                var match = new Match
                {
                    status = "yet to play",
                    sport_id = sportManaged.id // Assuming sport id needs to be set
                };
                db.Matches.Add(match);
                db.SaveChanges();

                // Log the match ID for debugging purposes
                var matchId = match.id;
                System.Diagnostics.Debug.WriteLine($"New match created with ID: {matchId}");

                // Update the schedule's match ID
                schedule.match_id = match.id;

                // Add the schedule
                db.Schedules.Add(schedule);
                db.SaveChanges();

                // Log the schedule ID for debugging purposes
                var scheduleId = schedule.id;
                System.Diagnostics.Debug.WriteLine($"New schedule created with ID: {scheduleId}");

                return Request.CreateResponse(HttpStatusCode.OK, schedule.id);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }




        [HttpDelete]
        public HttpResponseMessage DeleteSchedule(int id)
        {
            try
            {
                var original = db.Schedules.Find(id);
                if (original == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Schedule not found");
                }

                // Remove match ID from Matches table
                var matchId = original.match_id;
                var matchToRemove = db.Matches.FirstOrDefault(m => m.id == matchId);
                if (matchToRemove != null)
                {
                    db.Matches.Remove(matchToRemove);
                }

                // Delete schedule
                db.Entry(original).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "Schedule and associated match deleted");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.Message);
            }
        }


    }
}