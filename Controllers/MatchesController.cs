using SportsWeekManager.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace SportsWeekManager.API.Controllers
{
    public class MatchesController : ApiController
    {
        SportsManagementDBEntities db = new SportsManagementDBEntities();
        
        /*        Read All matches */
        [HttpGet]
        public HttpResponseMessage getMatch()
        {
            try
            {

                var matches = from m in db.Matches
                              join s in db.Schedules on m.id equals s.match_id
                              join t1 in db.Teams on s.team1_id equals t1.id
                              join t2 in db.Teams on s.team2_id equals t2.id
                              join sp in db.Sports on m.sport_id equals sp.id
                              join v in db.venues on s.id equals v.schedule_id
                              join e in db.Events on sp.event_id equals e.id
                              select new
                              {
                                  MatchId = m.id,
                                  Team1Name = t1.name,
                                  FirstHalfScore = m.first_half_score,
                                  Team2Name = t2.name,
                                  SecondHalfScore = m.second_half_score,
                                  Status = m.status,
                                  SportName = sp.name,
                                  Round = m.round,
                                  Date = s.date,
                                  Time = s.time,
                                  VenueName = v.name,
                                  EventName = e.eventname,
                                  Year = e.year
                              };

                return Request.CreateResponse(HttpStatusCode.OK, matches);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
       
        /*Create match*/
        [HttpPost]
        public HttpResponseMessage addMatch(Match match)
        {
            try
            {
                var matchlist = db.Matches.Add(match);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, match.id);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /* public HttpResponseMessage AddMatch(Match match, int userId)
         {
             try
             {
                 // Check if the user is an event manager
                 var user = db.Users.FirstOrDefault(u => u.id == userId && u.role == "event manager");
                 if (user == null)
                 {
                     return Request.CreateResponse(HttpStatusCode.OK, "User is not authorized to add matches.");
                 }

                 // Check if the user is assigned to the specific sport
                 var sport = db.Sports.FirstOrDefault(s => s.id == match.sport_id && s.user_id == userId);
                 if (sport == null)
                 {
                     return Request.CreateResponse(HttpStatusCode.Forbidden, "User is not authorized to add matches for this sport.");
                 }

                 // Add the match to the database
                 db.Matches.Add(match);
                 db.SaveChanges();

                 // Get the first related schedule (if available)
                 //var firstSchedule = match.Schedules.FirstOrDefault();

                 // Add the match to the schedule
                 if (firstSchedule != null)
                 {
                     Schedule newSchedule = new Schedule
                     {
                         team1_id = firstSchedule.team1_id,
                         team2_id = firstSchedule.team2_id,
                         match_id = match.id,
                         date = firstSchedule.date,
                         time = firstSchedule.time
                     };

                     db.Schedules.Add(newSchedule);
                     db.SaveChanges();
                 }

                 return Request.CreateResponse(HttpStatusCode.OK, match.id);
             }
             catch (Exception ex)
             {
                 return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
             }
         }*/

        /*Update match */
        [HttpPost]
        public HttpResponseMessage UpdateMatch(Match match)
        {
            try
            {
                var original = db.Matches.Find(match.id);
                if (original == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Match not found");
                }


                // Update only if the field is provided
                if (match.first_half_score != null) original.first_half_score = match.first_half_score;
                if (match.second_half_score != null) original.second_half_score = match.second_half_score;
                if (match.status != null) original.status = match.status;
                if (match.sport_id != null) original.sport_id = match.sport_id; // Updated condition for sport_id
                if (match.round != null) original.round = match.round;

                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Match Updated");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }




        /*remove match*/
        [HttpDelete]
        public HttpResponseMessage removeMatch(int id)
        {
            try
            {
                var original = db.Matches.Find(id);
                if (original == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "match not Found");
                }
                db.Entry(original).State=System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "match is removed");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /*Search match*/
        [HttpGet]
        public HttpResponseMessage searchMatch(string teamName)
        {
            try
            {
                var original = db.Matches.Find(teamName);
                return Request.CreateResponse(HttpStatusCode.OK, original);
                
               /* if (teamName == "all")
                {
                    var allmatches = db.Matches.ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, allmatches);
                }
                else 
                {
                    var match = db.Matches.Where(m => m.Sport.name == teamName);
                    return Request.CreateResponse(HttpStatusCode.OK, match);

                }*/
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


    }
}