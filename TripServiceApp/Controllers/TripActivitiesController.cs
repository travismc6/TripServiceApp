using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using TripServiceApp.Models;

namespace TripServiceApp.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TripActivitiesController : ApiController
    {
        private TripServiceAppContext db = new TripServiceAppContext();

        // GET: api/TripActivities
        [HttpGet]
        [Route("api/TripActivities/{id}")]
        public IQueryable<TripActivity> GetTripActivitiesByTrip(int id)
        {
            return db.TripActivities.Where(r=> r.TripId == id).Include("ActivityVotes");
        }

        // GET: api/TripActivities
        public IQueryable<TripActivity> GetTripActivities()
        {
            return db.TripActivities;
        }

        // GET: api/TripActivities/5
        [ResponseType(typeof(TripActivity))]
        public IHttpActionResult GetTripActivity(int id)
        {
            TripActivity tripActivity = db.TripActivities.Find(id);
            if (tripActivity == null)
            {
                return NotFound();
            }

            return Ok(tripActivity);
        }

        [HttpPut]
        [ResponseType(typeof(void))]
        [Route("api/TripActivities/UpdateActivityLocation/{id}/{location}")]
        public IHttpActionResult UpdateActivityLocation(int id, string location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TripActivity activity = db.TripActivities.FirstOrDefault(r => r.Id == id);

            if (activity == null)
            {
                return BadRequest();
            }

            activity.Location = location;

            db.Entry(activity).State = EntityState.Modified;


            try
            {
                db.SaveChanges();

                return StatusCode(HttpStatusCode.OK);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripActivityExists(id))
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

        [HttpPut]
        [ResponseType(typeof(void))]
        [Route("api/TripActivities/{id}/Complete/{isComplete}")]
        public IHttpActionResult CompleteActivity(int id, bool isComplete)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TripActivity activity = db.TripActivities.FirstOrDefault(r => r.Id == id);

            if (activity == null)
            {
                return BadRequest();
            }

            activity.IsComplete = isComplete;

            db.Entry(activity).State = EntityState.Modified;
           

            try
            {
                db.SaveChanges();

                return StatusCode(HttpStatusCode.OK);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripActivityExists(id))
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

        // PUT: api/TripActivities/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTripActivity(int id, TripActivity tripActivity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tripActivity.Id)
            {
                return BadRequest();
            }

            db.Entry(tripActivity).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripActivityExists(id))
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

        // POST: api/TripActivities
        [ResponseType(typeof(TripActivity))]
        public IHttpActionResult PostTripActivity(TripActivity tripActivity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //TripComment comment = new TripComment();
            //comment.Date = DateTime.UtcNow;
            //comment.TripUserId = tripActivity.TripUserId;
            //comment.Comment = "suggested an activity - " + tripActivity.Activity;

            //db.TripComments.Add(comment);

            db.TripActivities.Add(tripActivity);
            db.SaveChanges();

            var tripUsers = db.TripUsers.Where(r => r.Id == tripActivity.TripUserId).Select(r => r.Id).ToList();

            var notifyIds = db.PushRegistrations.Where(r => tripUsers.Contains(r.TripUserId)).Select(r => r.RegistrationId);

            var tripUser = db.TripUsers.Include("Trip").FirstOrDefault(r => r.Id == tripActivity.TripUserId);

            foreach (string notifyId in notifyIds)
            {
                var unused = Common.SendGms(notifyId, tripUser.Id.ToString(), tripUser.TripId.ToString(), tripUser.DisplayName + " suggested an Activity!", tripActivity.Activity,
                    "activities", tripUser.Trip.Code, tripUser.TripCode, tripUser.DisplayName, tripUser.Trip.Destination);
            }

            foreach (var tu in db.TripUsers.Where(r => r.TripId == tripActivity.TripId))
            {

                var message = tripUser.DisplayName + " suggested a new Activity for your trip to " + tripUser.Trip.Destination + "!";
                message += "\ntripsieappweb.azurewebsites.net/Trip/ActivityDetails/" + tripActivity.Id + "/" + "?code=" + tu.TripCode;

                if (tu.Phone != null)
                {
                    Common.SendSms(tu.Phone, message);
                }
                
            }

            return CreatedAtRoute("DefaultApi", new { id = tripActivity.Id }, tripActivity);
        }

        // DELETE: api/TripActivities/5
        [ResponseType(typeof(TripActivity))]
        public IHttpActionResult DeleteTripActivity(int id)
        {
            TripActivity tripActivity = db.TripActivities.Find(id);
            if (tripActivity == null)
            {
                return NotFound();
            }

            db.TripActivities.Remove(tripActivity);
            db.SaveChanges();

            return Ok(tripActivity);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TripActivityExists(int id)
        {
            return db.TripActivities.Count(e => e.Id == id) > 0;
        }
    }
}