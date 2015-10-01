using PushSharp;
using PushSharp.Android;
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
    public class TripUsersController : ApiController
    {
        private TripServiceAppContext db = new TripServiceAppContext();

        // GET: api/TripUsers
        public IQueryable<TripUser> GetTripUsers()
        {
            return db.TripUsers;
        }

        // Put: api/Trips/UserCode
        [ResponseType(typeof(void))]
        [HttpGet]
        [Route("api/TripUsers/ResendCode/{phone}")]
        public IHttpActionResult ResendCode(string phone)
        {


            var tripUsers = db.TripUsers.Where(r => r.Phone.Replace(" ", String.Empty).Replace("-", String.Empty).Replace("#", "").Replace("(", "").Replace(")", "") == phone.Replace(" ", String.Empty).Replace("-", String.Empty).Replace("#", "").Replace("(", "").Replace(")", ""));

            foreach(var tu in tripUsers)
            {
                var message = "Your Tripsie code is " + tu.TripCode;
                message += "\nteambitewolf.azurewebsites.net/Detail/" + tu.TripCode;

                Common.SendSms(tu.Phone, message);            }

            return StatusCode(HttpStatusCode.OK);
        }

                // Put: api/Trips/UserCode
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/TripUsers/{id}/name/{name}")]
        public TripUser UpdateUserDisplayName(int id, string name)
        {
            TripUser user = db.TripUsers.FirstOrDefault(r => r.Id == id);

            if(user == null)
            {
                return null;
            }

            if(user.Phone != null)
                user.DisplayName = name;
                // user.DisplayName += name + " (" + user.Phone.ToString() + ")";

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return user;
        }

        // Put: api/Trips/UserCode
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("api/TripUsers/Response/{id}/{response}/{comment}")]
        public IHttpActionResult UpdateTripUserResponse(string id, bool response, string comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TripUser tripUser = db.TripUsers.Include("Trip").FirstOrDefault(r => r.TripCode == id);

            if (tripUser == null)
            {
                return BadRequest();
            }

            tripUser.TripStatus = response == true ? TripUserStatus.Yes : TripUserStatus.No;

            db.Entry(tripUser).State = EntityState.Modified;

            TripComment tripComment = new TripComment();
            tripComment.Date = DateTime.Now.ToUniversalTime();

            String notificationText = tripUser.DisplayName + " is ";


            if(response)
            {
                tripComment.Comment = "I'm IN!";
                notificationText += "IN for "; 
            }

            else
            {
                tripComment.Comment = "I'm OUT.";
                notificationText += "OUT of ";
            }

            TripComment responseComment = null;

            if(!String.IsNullOrEmpty(comment) && comment != "-1111")
            {
                responseComment = new TripComment();

                responseComment.Comment = comment;
                responseComment.Date = DateTime.UtcNow;
            }

            notificationText += "the trip to " + tripUser.Trip.Destination;

            tripUser.TripComments = new List<TripComment>();
            tripUser.TripComments.Add(tripComment);

            if(responseComment != null)
            {
                tripUser.TripComments.Add(responseComment);
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripUserExists(tripUser.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var tripUsers = tripUser.Trip.Users.Select(r => r.Id);

            var notifyIds = db.PushRegistrations.Where(r => tripUsers.Contains(r.TripUserId)).Select(r => r.RegistrationId);

            foreach (string notifyId in notifyIds)
            {
               var unused = Common.SendGms(notifyId, tripUser.Id.ToString(), tripUser.TripId.ToString(), "Tripsie Update!", notificationText, "details", tripUser.Trip.Code, tripUser.TripCode);
            }

            return StatusCode(HttpStatusCode.OK);
        }


        // Put: api/TripUsers/UpdateLocation/2312.213/123123.2
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("api/TripUsers/UpdateLocation/{id}/{lat}/{lon}/")]
        public IHttpActionResult UpdateTripUserLocation(string id, string lat, string lon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TripUser tripUser = db.TripUsers.Include("Trip").FirstOrDefault(r => r.TripCode == id);

            if (tripUser == null)
            {
                return BadRequest();
            }

            tripUser.Lat = Double.Parse( lat);
            tripUser.Lon = Double.Parse(lon);

            db.Entry(tripUser).State = EntityState.Modified;


            String notificationText = tripUser.DisplayName + " has an updated location!";


            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripUserExists(tripUser.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var tripUsers = tripUser.Trip.Users.Select(r => r.Id);

            var notifyIds = db.PushRegistrations.Where(r => tripUsers.Contains(r.TripUserId)).Select(r => r.RegistrationId);

            //notifyIds = db.PushRegistrations.Select(r => r.RegistrationId);

            foreach (string notifyId in notifyIds)
            {
                var unused = Common.SendGms(notifyId, tripUser.Id.ToString(), tripUser.TripId.ToString(), "Tripsie Update!", notificationText, "location", tripUser.Trip.Code, tripUser.TripCode);
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // GET: api/TripUsers/5
        [ResponseType(typeof(TripUser))]
        public IHttpActionResult GetTripUser(int id)
        {
            TripUser tripUser = db.TripUsers.Find(id);
            if (tripUser == null)
            {
                return NotFound();
            }

            return Ok(tripUser);
        }

        // PUT: api/TripUsers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTripUser(int id, TripUser tripUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tripUser.Id)
            {
                return BadRequest();
            }

            db.Entry(tripUser).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripUserExists(id))
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

        // POST: api/TripUsers
        [ResponseType(typeof(TripUser))]
        public IHttpActionResult PostTripUser(TripUser tripUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TripUsers.Add(tripUser);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = tripUser.Id }, tripUser);
        }

        // DELETE: api/TripUsers/5
        [ResponseType(typeof(TripUser))]
        public IHttpActionResult DeleteTripUser(int id)
        {
            TripUser tripUser = db.TripUsers.Find(id);
            if (tripUser == null)
            {
                return NotFound();
            }

            db.TripUsers.Remove(tripUser);
            db.SaveChanges();

            return Ok(tripUser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TripUserExists(int id)
        {
            return db.TripUsers.Count(e => e.Id == id) > 0;
        }
    }
}