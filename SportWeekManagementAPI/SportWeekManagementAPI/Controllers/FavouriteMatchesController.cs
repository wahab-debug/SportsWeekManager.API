using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SportWeekManagementAPI.Models;

namespace SportWeekManagementAPI.Controllers
{
    public class FavouriteMatchesController : ApiController
    {
        SportsWeekManagementEntities db = new SportsWeekManagementEntities();
        
        /*get all favourite list of user*/
        [HttpGet]
        public HttpResponseMessage getFavlist(int id)
        {
            try
            {
                var favlist = db.favourites
            .Where(f => f.user_id == id)
            .Join(
                db.Matches,
                f => f.match_id,
                m => m.id,
                (f, m) => new { Favourite = f, Match = m }
            )
            .Join(
                db.Schedules,
                fm => fm.Match.id,
                s => s.match_id,
                (fm, s) => new { fm.Favourite, fm.Match, Schedule = s }
            )
            .Join(
                db.Teams,
                fms => fms.Schedule.team1_id,
                t1 => t1.id,
                (fms, t1) => new { fms.Favourite, fms.Match, fms.Schedule, Team1Name = t1.name }
            )
            .Join(
                db.Teams,
                fmst1 => fmst1.Schedule.team2_id,
                t2 => t2.id,
                (fmst1, t2) => new { fmst1.Favourite, fmst1.Match, fmst1.Schedule, fmst1.Team1Name, Team2Name = t2.name }
            )
            .Join(
                db.Sports,
                fmst1 => fmst1.Match.sport_id,
                s => s.id,
                (fmst1, sport) => new {
                    FavouriteId = fmst1.Favourite.id,
                    fmst1.Match.first_half_score,
                    fmst1.Match.second_half_score,
                    fmst1.Match.status,
                    fmst1.Match.round,
                    fmst1.Team1Name,
                    fmst1.Team2Name,
                    fmst1.Schedule.date,
                    fmst1.Schedule.time,
                    SportName = sport.name
                }
            )
            .ToList();

                if (!favlist.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No favourites found.");
                }

                return Request.CreateResponse(HttpStatusCode.OK, favlist);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        /*add to favourite list*/
        [HttpPost]
        public HttpResponseMessage AddFavourite(favourite favourite)
        {
            try
            {
                // Check if the favourite object is null
                if (favourite == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Favourite data cannot be null.");
                }

                // Check if the favourite already exists
                var existingFavourite = db.favourites
                    .FirstOrDefault(f => f.user_id == favourite.user_id && f.match_id == favourite.match_id);

                if (existingFavourite != null)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, "This match is already favourited by the user.");
                }

                // Add the favourite to the database
                db.favourites.Add(favourite);
                db.SaveChanges();

                // Return success response
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Favourite added successfully", favouriteId = favourite.id });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        /*remove to favourite list*/
        [HttpDelete]
        public HttpResponseMessage RemoveFav(int userId, int matchId)
        {
            try
            {
                var favourite = db.favourites.SingleOrDefault(f => f.user_id == userId && f.match_id == matchId);

                if (favourite == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Favorite not found.");
                }

                db.favourites.Remove(favourite);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "Favorite removed successfully.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
