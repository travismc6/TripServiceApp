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
    public class PushRegistrationsController : ApiController
    {
        private TripServiceAppContext db = new TripServiceAppContext();

        // GET: api/PushRegistrations
        public IQueryable<PushRegistrations> GetPushRegistrations()
        {
            return db.PushRegistrations;
        }

        // GET: api/PushRegistrations/5
        [ResponseType(typeof(PushRegistrations))]
        public IHttpActionResult GetPushRegistrations(int id)
        {
            PushRegistrations pushRegistrations = db.PushRegistrations.Find(id);
            if (pushRegistrations == null)
            {
                return NotFound();
            }

            return Ok(pushRegistrations);
        }

        // PUT: api/PushRegistrations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPushRegistrations(int id, PushRegistrations pushRegistrations)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pushRegistrations.Id)
            {
                return BadRequest();
            }


            db.Entry(pushRegistrations).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PushRegistrationsExists(id))
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

        // POST: api/PushRegistrations
        [ResponseType(typeof(PushRegistrations))]
        public IHttpActionResult PostPushRegistrations(PushRegistrations pushRegistrations)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            else if (db.PushRegistrations.Any(r => r.TripUserId == pushRegistrations.TripUserId && r.RegistrationId == pushRegistrations.RegistrationId))
            {
                return null;
            }

            db.PushRegistrations.Add(pushRegistrations);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = pushRegistrations.Id }, pushRegistrations);
        }

        // DELETE: api/PushRegistrations/5
        [ResponseType(typeof(PushRegistrations))]
        public IHttpActionResult DeletePushRegistrations(int id)
        {
            PushRegistrations pushRegistrations = db.PushRegistrations.Find(id);
            if (pushRegistrations == null)
            {
                return NotFound();
            }

            db.PushRegistrations.Remove(pushRegistrations);
            db.SaveChanges();

            return Ok(pushRegistrations);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PushRegistrationsExists(int id)
        {
            return db.PushRegistrations.Count(e => e.Id == id) > 0;
        }
    }
}