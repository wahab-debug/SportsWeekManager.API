using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SportWeekManagementAPI.Models;

namespace SportWeekManagementAPI.Controllers
{
    public class PlayerTeamInfoController : ApiController
    {
        SportsWeekManagementEntities db = new SportsWeekManagementEntities();

        [HttpPost]
        public HttpResponseMessage AddPlayers(List<Playerteaminfo> players)
        {
            try
            {
                var teamId = players.FirstOrDefault()?.team_id;
                if (teamId == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Team ID is required.");
                }

                int currentPlayerCount = db.Playerteaminfoes.Count(p => p.team_id == teamId);

                if (currentPlayerCount + players.Count > 12)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Team cannot have more than 12 players.");
                }

                db.Playerteaminfoes.AddRange(players);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "Players added successfully.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


    }
}
