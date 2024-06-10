using SportsWeekManager.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SportsWeekManager.API.Controllers
{
    public class CommentController : ApiController
    {
        SportsManagementDBEntities db = new SportsManagementDBEntities();
        [HttpGet]
        public HttpResponseMessage GetComments(int matchId)
        {
            try
            {
                var comments = db.Comments.Where(c => c.match_id == matchId).OrderBy(c => c.id).ToList();

                if (comments.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, comments);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No comments found for the specified match ID.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage AddComment(int userId, Comment comment)
        {
            try
            {
                // Check if the user is an event manager
                var user = db.Users.FirstOrDefault(u => u.id == userId && u.role == "event manager");
                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Only event managers can add comments.");
                }

                // Retrieve the sport managed by the event manager
                var sportManaged = db.Sports.FirstOrDefault(s => s.user_id == userId);
                if (sportManaged == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User does not manage any sport.");
                }

                // Check if the user is affiliated with the relevant sport for the match
                var match = db.Matches.FirstOrDefault(m => m.id == comment.match_id);
                if (match == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Match not found.");
                }

                var matchSport = db.Sports.FirstOrDefault(s => s.id == match.sport_id);
                if (matchSport == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Sport of the match not found.");
                }

                // Check if the event manager manages the sport of the match
                if (sportManaged.id != matchSport.id)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Event manager does not manage the sport of the match.");
                }

                // Add the comment
                db.Comments.Add(comment);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "Comment added successfully.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public HttpResponseMessage UpdateComment(int userId, int commentId, Comment updatedComment)
        {
            try
            {
                // Check if the user is an event manager
                var user = db.Users.FirstOrDefault(u => u.id == userId && u.role == "event manager");
                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Only event managers can update comments.");
                }

                // Retrieve the comment
                var comment = db.Comments.FirstOrDefault(c => c.id == commentId);
                if (comment == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Comment not found.");
                }

                // Retrieve the sport managed by the event manager
                var sportManaged = db.Sports.FirstOrDefault(s => s.user_id == userId);
                if (sportManaged == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User does not manage any sport.");
                }

                // Retrieve the match associated with the comment
                var match = db.Matches.FirstOrDefault(m => m.id == comment.match_id);
                if (match == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Match associated with the comment not found.");
                }

                var matchSport = db.Sports.FirstOrDefault(s => s.id == match.sport_id);
                if (matchSport == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Sport of the match not found.");
                }

                // Check if the event manager manages the sport of the match
                if (sportManaged.id != matchSport.id)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Event manager does not manage the sport of the match.");
                }

                // Update the comment
                comment.description = updatedComment.description;
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "Comment updated successfully.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}