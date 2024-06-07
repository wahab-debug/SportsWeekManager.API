using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SportWeekManagementAPI.Models;

namespace SportWeekManagementAPI.Controllers
{
    public class RulesController : ApiController
    {
        SportsWeekManagementEntities db = new SportsWeekManagementEntities();

        [HttpPost]

        public HttpResponseMessage AddRules(string sportName, string rules)
        {
            try
            {
                // Find the sport by name
                var sport = db.Sports.FirstOrDefault(s => s.name == sportName);

                if (sport == null)
                {
                    // If the sport is not found
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Sport not found");
                }

                if (string.IsNullOrEmpty(sport.rule))
                {
                    // If the rules are currently null or empty
                    sport.rule = rules;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "Rules added");
                }
                else
                {
                    // If rules already exist, update them
                    sport.rule = rules;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "Rules modified");
                }
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}
