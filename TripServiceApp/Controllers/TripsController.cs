using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using TripServiceApp.Models;

namespace TripServiceApp.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TripsController : ApiController
    {
        private TripServiceAppContext db = new TripServiceAppContext();
        Random random = new Random();

        // GET: api/Trips
        public IQueryable<Trip> GetTrips()
        {
            return db.Trips.Include("Users");
        }

        [ResponseType(typeof(Trip))]
        [Route("api/Trips/{id}/start/{start}")]
        [HttpPut]
        public IHttpActionResult UpdateTripStart(int  id, string start)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Trip trip = db.Trips.FirstOrDefault(r => r.Id == id);
            trip.StartDate = start.Replace('-', '/');

            var tripUser = db.TripUsers.Where(r=> r.TripId == id).FirstOrDefault(r => r.IsCreator);

            if (trip == null)
            {
                return NotFound();
            }

            db.Entry(trip).State = EntityState.Modified;

            String notificationText = tripUser.DisplayName + " updated your trip to " + trip.Destination + "!";


            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var tripUsers = trip.Users.Select(r => r.Id);

            var notifyIds = db.PushRegistrations.Where(r => tripUsers.Contains(r.TripUserId)).Select(r => r.RegistrationId);

            //notifyIds = db.PushRegistrations.Select(r => r.RegistrationId);

            foreach (string notifyId in notifyIds)
            {
                var unused = Common.SendGms(notifyId, tripUser.Id.ToString(), tripUser.TripId.ToString(), "Tripsie Update!", notificationText, "location", tripUser.Trip.Code, tripUser.TripCode);
            }

            return Ok(trip);
        }

        [ResponseType(typeof(Trip))]
        [Route("api/Trips/{id}/end/{end}")]
        [HttpPut]
        public IHttpActionResult UpdateTripEnd(int id, string end)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            Trip trip = db.Trips.FirstOrDefault(r => r.Id == id);
            trip.EndDate = end.Replace('-', '/');

            var tripUser = db.TripUsers.Where(r => r.TripId == id).FirstOrDefault(r => r.IsCreator);

            if (trip == null)
            {
                return NotFound();
            }

            db.Entry(trip).State = EntityState.Modified;

            String notificationText = tripUser.DisplayName + " updated your trip to " + trip.Destination + "!";


            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var tripUsers = trip.Users.Select(r => r.Id);

            var notifyIds = db.PushRegistrations.Where(r => tripUsers.Contains(r.TripUserId)).Select(r => r.RegistrationId);

           // notifyIds = db.PushRegistrations.Select(r => r.RegistrationId);

            foreach (string notifyId in notifyIds)
            {
                var unused = Common.SendGms(notifyId, tripUser.Id.ToString(), tripUser.TripId.ToString(), "Tripsie Update!", notificationText, "details", tripUser.Trip.Code, tripUser.TripCode);
            }

            return Ok(trip);
        }

        // GET: api/Trips/UserCode
        [ResponseType(typeof(Trip))]
        [Route("api/UserCode/{id}")]
        public IHttpActionResult GetTripFromUserCode(string id)
        {

            TripUser tripUser = db.TripUsers.FirstOrDefault(r => r.TripCode == id);
            if (tripUser == null)
            {
                return NotFound();
            }

            Trip trip = db.Trips.FirstOrDefault(r => r.Id == tripUser.TripId);

            if (trip == null)
            {
                return NotFound();
            }

            return Ok(trip);
        }

        // GET: api/Trips/ABCDE
        [ResponseType(typeof(Trip))]
        public IHttpActionResult GetTrip(string id)
        {
            Trip trip = db.Trips.FirstOrDefault(r=> r.Code == id);
            if (trip == null)
            {
                return NotFound();
            }

            return Ok(trip);
        }

        // PUT: api/Trips/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTrip(int id, Trip trip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != trip.Id)
            {
                return BadRequest();
            }

            db.Entry(trip).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripExists(id))
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

        // POST: api/Trips
        [ResponseType(typeof(Trip))]
        public IHttpActionResult PostTrip(Trip trip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            trip.Users = new List<TripUser>();
            trip.Status = TripStatus.Open;
            trip.Code = GetCode(8);

            TripUser userCreator = new TripUser();
            userCreator.IsCreator = true;
            userCreator.TripId = trip.Id;
            userCreator.DisplayName = trip.MyName;
            userCreator.TripCode = GetCode(5);
            userCreator.TripStatus = TripUserStatus.Yes;
            trip.Users.Add(userCreator);

            var userJson =  JsonConvert.DeserializeObject<Users>(trip.UserJson);

            foreach(TripUser user in userJson.users)
            {
                user.TripStatus = TripUserStatus.Pending;
                user.TripCode = GetCode(5);
                user.IsCreator = false;

                trip.Users.Add(user);
            }

            db.Trips.Add(trip);
            db.SaveChanges();

            var website = "teambitewolf.azurewebsites.net/Detail/";

            // send out text invite
            var smsMessage1 = String.Format("Let's Tripsie! {0} send you an invite for a Trip to {1}", trip.MyName, trip.Destination);

            var smsMessage2 =
                "Visit " + Common.WebsiteDomain + "/Detail/{0} or download the Tripsie app for Android to get started!";

            foreach (var tu in trip.Users)
            {
                if (tu.Phone != null)
                {
                    // @TODO don't send second message until first one complete
                    Common.SendSms(tu.Phone, smsMessage1);
                    Common.SendSms(tu.Phone, string.Format(smsMessage2, tu.TripCode));
                }
            }


            return CreatedAtRoute("DefaultApi", new { id = trip.Id }, trip);
        }

        private string GetCode(int size)
        {
            string code = "";

            char[] chars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                char c = chars[random.Next(chars.Length)];
                sb.Append(c);
            }

            return sb.ToString().ToUpper();
        }

        // DELETE: api/Trips/5
        [ResponseType(typeof(Trip))]
        public IHttpActionResult DeleteTrip(int id)
        {
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return NotFound();
            }

            db.Trips.Remove(trip);
            db.SaveChanges();

            return Ok(trip);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TripExists(int id)
        {
            return db.Trips.Count(e => e.Id == id) > 0;
        }
    }

    public class Users
    {
        public List<TripUser> users { get; set; }
    }
}