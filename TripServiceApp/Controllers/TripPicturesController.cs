using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TripServiceApp.Models;

namespace TripServiceApp.Controllers
{
    public class TripPicturesController : ApiController
    {
        private TripServiceAppContext db = new TripServiceAppContext();

        // GET: api/TripPictures
        public IQueryable<TripPicture> GetTripPictures()
        {

            return db.TripPictures;
        }

        // GET: api/TripPictures/5
        [ResponseType(typeof(TripPicture))]
        public IHttpActionResult GetTripPicture(int id)
        {
            TripPicture tripPicture = db.TripPictures.Find(id);
            if (tripPicture == null)
            {
                return NotFound();
            }

            return Ok(tripPicture);
        }

        // PUT: api/TripPictures/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTripPicture(int id, TripPicture tripPicture)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tripPicture.Id)
            {
                return BadRequest();
            }

            db.Entry(tripPicture).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripPictureExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/TripPictures
        [ResponseType(typeof(TripPicture))]
        public IHttpActionResult PostTripPicture(TripPicture tripPicture)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TripPictures.Add(tripPicture);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = tripPicture.Id }, tripPicture);
        }

        // DELETE: api/TripPictures/5
        [ResponseType(typeof(TripPicture))]
        public IHttpActionResult DeleteTripPicture(int id)
        {
            TripPicture tripPicture = db.TripPictures.Find(id);
            if (tripPicture == null)
            {
                return NotFound();
            }

            db.TripPictures.Remove(tripPicture);
            db.SaveChanges();

            return Ok(tripPicture);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TripPictureExists(int id)
        {
            return db.TripPictures.Count(e => e.Id == id) > 0;
        }
    }
}