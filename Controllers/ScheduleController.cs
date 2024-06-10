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
        [HttpPost]
        public HttpResponseMessage UpdateMatch(int userId, Match match)
        {
            try
            {
                var original = db.Matches.Find(match.id);
                if (original == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Match not found");
                }

                // Retrieve the sport managed by the event manager
                var sportManaged = db.Sports.FirstOrDefault(s => s.user_id == userId);
                if (sportManaged == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User does not manage any sport.");
                }

                // Retrieve the sport of the match being updated
                var matchSport = db.Sports.FirstOrDefault(s => s.id == original.sport_id);
                if (matchSport == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Sport of the match not found.");
                }

                // Check if the event manager manages the sport of the match
                if (sportManaged.id != matchSport.id)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Event manager does not manage the sport of the match.");
                }

                // Update only if the field is provided
                if (match.first_half_score != null) original.first_half_score = match.first_half_score;
                if (match.second_half_score != null) original.second_half_score = match.second_half_score;
                if (match.status != null) original.status = match.status;
                if (match.round != null) original.round = match.round;

                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Match Updated");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
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