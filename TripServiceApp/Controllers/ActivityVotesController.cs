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
    public class ActivityVotesController : ApiController
    {
        public static object lockObj = new object();

        private TripServiceAppContext db = new TripServiceAppContext();

        // GET: api/ActivityVotes
        public IQueryable<ActivityVote> GetActivityVotes()
        {
            return db.ActivityVotes;
        }

        // GET: api/ActivityVotes/5
        [ResponseType(typeof(ActivityVote))]
        public IHttpActionResult GetActivityVote(int id)
        {
            ActivityVote activityVote = db.ActivityVotes.Find(id);
            if (activityVote == null)
            {
                return NotFound();
            }

            return Ok(activityVote);
        }

        // PUT: api/ActivityVotes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutActivityVote(int id, ActivityVote activityVote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != activityVote.Id)
            {
                return BadRequest();
            }

            db.Entry(activityVote).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityVoteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = activityVote.Id }, activityVote);
        }

        // POST: api/ActivityVotes
        [ResponseType(typeof(ActivityVote))]
        public IHttpActionResult PostActivityVote(ActivityVote activityVote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var context = new TripServiceAppContext();

            lock (lockObj)
            {
                ActivityVote vote = context.ActivityVotes.Where(r => r.TripActivityId == activityVote.TripActivityId && r.TripUserId == activityVote.TripUserId).FirstOrDefault();

                // user had already voted. update their vote
                if (vote != null)
                {
                    vote.Vote = activityVote.Vote;

                    return PutActivityVote(vote.Id, vote); ;
                }

                else
                {
                    context.ActivityVotes.Add(activityVote);
                    context.SaveChanges();

                    return CreatedAtRoute("DefaultApi", new { id = activityVote.Id }, activityVote);
                }
            }
        }

        // DELETE: api/ActivityVotes/5
        [ResponseType(typeof(ActivityVote))]
        public IHttpActionResult DeleteActivityVote(int id)
        {
            ActivityVote activityVote = db.ActivityVotes.Find(id);
            if (activityVote == null)
            {
                return NotFound();
            }

            db.ActivityVotes.Remove(activityVote);
            db.SaveChanges();

            return Ok(activityVote);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ActivityVoteExists(int id)
        {
            return db.ActivityVotes.Count(e => e.Id == id) > 0;
        }
    }
}