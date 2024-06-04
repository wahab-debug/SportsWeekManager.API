using SportWeekManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SportWeekManagementAPI.Controllers
{
    public class UserController : ApiController
    {
        SportsManagementDBEntities db = new SportsManagementDBEntities();

        [HttpGet]

        public HttpResponseMessage Login(String username, String password)
        {
            try
            {
                var user = db.Users.FirstOrDefault(v => v.name == username && v.password == password);

                if (user == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid Username or password");
                }

                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [HttpPost]
        public HttpResponseMessage ChangePassword(int userId, String oldPassword, String newPassword)
        {
            try
            {
                var user = db.Users.FirstOrDefault(v => v.id == userId && v.password == oldPassword);

                if (user == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid userId or password");
                }

                user.password = newPassword;
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "Password changed successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


    }
}
