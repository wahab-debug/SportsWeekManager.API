using SportsWeekManager.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SportsWeekManager.API.Controllers
{
    public class FavouriteMatchesController : ApiController
    {
        SportsManagementDBEntities db = new SportsManagementDBEntities();
        /*get all favourite list of user*/
        [HttpGet]
        public HttpResponseMessage getFavlist(int id)
        {

            try 
            {
               
                var favlist = db.favourites
                    .Where(f => f.user_id == id)
                    .Join(db.Matches,
                          f => f.match_id,
                          m => m.id,
                          (f, m) => new {
                              m.first_half_score,
                              m.second_half_score,
                              m.status,
                              m.round
                          })
                    .ToList();
                if (favlist == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, " not favourites");
                }
                return Request.CreateResponse(HttpStatusCode.OK,favlist);
            }
            catch(Exception ex)  
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        /*add to favourite list*/
        [HttpPost]
        public HttpResponseMessage addfav(favourite fav) 
        {
            try 
            {                
                db.favourites.Add(fav);
                // Save changes to the database
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, fav.id);
            }
            catch(Exception ex) 
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,ex.Message);
            }
        }
        

    }
}