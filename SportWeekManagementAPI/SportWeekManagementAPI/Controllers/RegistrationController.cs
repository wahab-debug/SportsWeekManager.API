using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SportWeekManagementAPI.Models;

namespace SportWeekManagementAPI.Controllers
{
    public class RegistrationController : ApiController
    {
        SportsWeekManagementEntities db = new SportsWeekManagementEntities();

        [HttpPost]
        public HttpResponseMessage ChangeStatus(int id, int status)
        {
            try
            {
                var team = db.Teams.Find(id);
                if (team == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Team not found");
                }

                if (status == 0)
                {
                    if (team.isRegistered == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Team not registered");
                    }
                    else
                    {
                        team.isRegistered = 0;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "Team status changed");
                    }
                }
                else if (status == 1)
                {
                    if (team.isRegistered == 1)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Team already registered");
                    }
                    else
                    {
                        team.isRegistered = 1;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "Team registered successfully");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid status value");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage ResetStatus()
        {
            try
            {
                var teams = db.Teams.ToList();

                foreach (var team in teams)
                {
                    team.isRegistered = 0;
                }

                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "All teams' registration status reset successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetTeamsByRegistrationStatus(int status)
        {
            try
            {
                if (status != 0 && status != 1)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid status value. Please provide 0 or 1.");
                }

                var teams = db.Teams.Where(t => t.isRegistered == status).Select(s => new
                {
                    s.id,
                    s.name,
                    s.semester,
                    s.isRegistered,
                    s.no_players

                })
                                         .OrderBy(b => b.id)
                                         .ToList();

                if (!teams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No teams found with the specified registration status.");
                }

                return Request.CreateResponse(HttpStatusCode.OK, teams);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }




    }
}
