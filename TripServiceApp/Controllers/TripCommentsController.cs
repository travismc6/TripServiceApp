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
    public class TripCommentsController : ApiController
    {
        private TripServiceAppContext db = new TripServiceAppContext();

        // GET: api/TripComments
        public IQueryable<TripComment> GetTripComments()
        {
            return db.TripComments.OrderByDescending(r=> r.Date);
        }

        // GET: api/TripComments/5
        [ResponseType(typeof(TripComment))]
        public IHttpActionResult GetTripComment(int id)
        {
            TripComment tripComment = db.TripComments.Find(id);
            if (tripComment == null)
            {
                return NotFound();
            }

            return Ok(tripComment);
        }

        // PUT: api/TripComments/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTripComment(int id, TripComment tripComment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tripComment.Id)
            {
                return BadRequest();
            }

            db.Entry(tripComment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripCommentExists(id))
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

        // POST: api/TripComments
        [ResponseType(typeof(TripComment))]
        public IHttpActionResult PostTripComment(TripComment tripComment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            tripComment.Date = DateTime.UtcNow;

            db.TripComments.Add(tripComment);
            db.SaveChanges();

            var tripUser = db.TripUsers.First(r=> r.Id == tripComment.TripUserId);


            var userIds = db.Trips.Include("Users").FirstOrDefault(r => r.Id == tripUser.TripId).Users.Select(r=>r.Id);

            var notifyIds = db.PushRegistrations.Where(r =>userIds.Contains(r.TripUserId)).Select(r => r.RegistrationId);

            foreach (string notifyId in notifyIds)
            {
                var unused = Common.SendGms(notifyId, tripUser.Id.ToString(), tripUser.TripId.ToString(), tripUser.DisplayName + " posted a comment!", tripComment.Comment, 
                    "comments",  tripUser.Trip.Code, tripUser.TripCode, tripUser.DisplayName, tripUser.Trip.Destination);
            }

            foreach (var tu in db.TripUsers.Where(r => r.TripId == tripUser.TripId))
            {
                string AccountSid = "ACbd60880fa81a594ce582f938d0716b8b";
                string AuthToken = "403c9bd50b0d252176ab553d4031ddd2";
                var twilio = new Twilio.TwilioRestClient(AccountSid, AuthToken);

                var message = tripUser.DisplayName + " posted a new ";

                if (tripComment.TripActivityId != null && tripComment.TripActivityId > 0)
                {
                    message += "Activity Comment for your trip to " + tripUser.Trip.Destination + "!";
                    message += "\ntripsieappweb.azurewebsites.net/Trip/ActivityDetails/" + tripComment.TripActivityId + "/?code=" + tu.TripCode;
                }

                else
                {
                    message += "Comment for your trip to " + tripUser.Trip.Destination + "!";
                    message += "\ntripsieappweb.azurewebsites.net/Trip/Comments/" + tu.TripCode;
                }


                if (tu.Phone != null)
                {
                    twilio.SendSmsMessage("+18666578771", CleanPhoneNumber(tu.Phone), message, null, (msg) => { });
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = tripComment.Id }, tripComment);
        }

        private string CleanPhoneNumber(string phone)
        {
            return phone.Replace(" ", String.Empty).Replace("-", String.Empty).Replace("#", "").Replace("(", "").Replace(")", "");
        }

        // DELETE: api/TripComments/5
        [ResponseType(typeof(TripComment))]
        public IHttpActionResult DeleteTripComment(int id)
        {
            TripComment tripComment = db.TripComments.Find(id);
            if (tripComment == null)
            {
                return NotFound();
            }

            db.TripComments.Remove(tripComment);
            db.SaveChanges();

            return Ok(tripComment);
        }

        // GET: api/Trips/UserCode
        [ResponseType(typeof(IQueryable<TripComment>))]
        [Route("api/TripComments/List/{id}")]
        public IQueryable<TripComment> GetTripCommentsByCode(int id)
        {


            return db.TripComments.Include("TripUser").Where(r => r.TripUser.TripId == id).OrderByDescending(r => r.Date) ;
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TripCommentExists(int id)
        {
            return db.TripComments.Count(e => e.Id == id) > 0;
        }
    }
}