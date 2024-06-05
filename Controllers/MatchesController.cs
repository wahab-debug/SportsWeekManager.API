using SportsWeekManager.API.Models;
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
                /*        Read all matches with name*/ 
        [HttpGet]
        public HttpResponseMessage getMatch() 
        {
            try 
            {
                var matches = db.Matches.Select(m=>new{

                    m.status,
                    m.first_half_score,
                    m.second_half_score,
                   
                    m.Sport.name
                 
                }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, matches);
            }
            catch(Exception ex) 
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