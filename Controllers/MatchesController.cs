﻿using SportsWeekManager.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SportsWeekManager.API.Controllers
{
    public class MatchesController : ApiController
    {
        SportsManagementDBEntities db = new SportsManagementDBEntities();
        /*        Read Cricet matches with scores against team name*/
        /* [HttpGet]
         public HttpResponseMessage getCricketMatch()
         {
             try
             {
                 var matches = db.Matches
              .Where(m => m.Sport.name == "Cricket") // Filter only cricket matches
              .Join(
                  db.Schedules,
                  m => m.id,
                  s => s.match_id,
                  (m, s) => new { Match = m, Schedule = s }
              )
              .Join(
                  db.Teams,
                  ms => ms.Schedule.team1_id,
                  t1 => t1.id,
                  (ms, t1) => new { ms.Match, ms.Schedule, Team1 = t1 }
              )
              .Join(
                  db.Teams,
                  mst1 => mst1.Schedule.team2_id,
                  t2 => t2.id,
                  (mst1, t2) => new
                  {
                      Status = mst1.Match.status,
                      FirstHalfScore = mst1.Match.first_half_score, // First half score belongs to Team1
                      SecondHalfScore = mst1.Match.second_half_score, // Second half score belongs to Team2
                      SportName = mst1.Match.Sport.name,
                      Team1Name = mst1.Team1.name,
                      Team2Name = t2.name,
                      mst1.Schedule.date,
                      mst1.Schedule.time
                  }
              )
              .ToList();

                 return Request.CreateResponse(HttpStatusCode.OK, matches);
             }
             catch (Exception ex)
             {
                 return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
             }
         }*/
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
        public HttpResponseMessage addmatch(Match match)
        {
            
                try
                {
                    db.Matches.Add(match);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, match.id);
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
        }

        /*Update match */
        [HttpPost]
        public HttpResponseMessage updateMatch(Match match) 
        {
            try 
            {
                
                var original = db.Matches.Find(match.id);
                if (original == null) 
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "match not Found");
                }
                db.Entry(original).CurrentValues.SetValues(match);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "match is modified");
            }
            catch (Exception ex) 
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
      
        /*remove match*/
        [HttpPost]
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