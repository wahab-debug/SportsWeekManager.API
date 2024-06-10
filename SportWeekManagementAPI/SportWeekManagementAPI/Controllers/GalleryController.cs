using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SportWeekManagementAPI.Models;

namespace SportWeekManagementAPI.Controllers
{
    public class GalleryController : ApiController
    {
        SportsWeekManagementEntities db = new SportsWeekManagementEntities();

        [HttpGet]
        public HttpResponseMessage GetGalleryItems()
        {
            try
            {
                var items = from gallery in db.galleries
                            select new
                            {
                                gallery.id,
                                gallery.path,
                                gallery.match_id,
                                gallery.Date
                            };

                return Request.CreateResponse(HttpStatusCode.OK, items.ToList());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        // Insert Gallery Item
        [HttpPost]
        public HttpResponseMessage PostGalleryItem(gallery galleryItem)
        {
            try
            {
                var result = db.galleries.Add(galleryItem);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // Update Gallery Item
        [HttpPut]
        public HttpResponseMessage PutGalleryItem(int id, gallery galleryItem)
        {
            try
            {
                var existingItem = db.galleries.FirstOrDefault(g => g.id == id);
                if (existingItem == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Gallery item not found");
                }

                existingItem.path = galleryItem.path;
                existingItem.match_id = galleryItem.match_id;
                existingItem.Date = galleryItem.Date;

                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Gallery item updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // Delete Gallery Item
        [HttpDelete]
        public HttpResponseMessage DeleteGalleryItem(int id)
        {
            try
            {
                var galleryItem = db.galleries.FirstOrDefault(g => g.id == id);
                if (galleryItem == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Gallery item not found");
                }

                db.galleries.Remove(galleryItem);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Gallery item deleted successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
