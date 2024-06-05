using SportsWeekManager.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SportsWeekManager.API.Controllers
{
    public class UserController : ApiController
    {
        SportsManagementDBEntities db = new SportsManagementDBEntities();

        [HttpGet]
        public IHttpActionResult GetData()
        {
            var list = db.Users
                .Select(u => new
                {
                    u.name,
                    u.registration_no,
                    u.password,
                    u.role
                })
                .ToList();

            return Ok(list);
        }
        [HttpPost]
        public HttpResponseMessage PostUser(User user)
        {
            try
            {
                var result = db.Users.Add(user);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }


    }
}