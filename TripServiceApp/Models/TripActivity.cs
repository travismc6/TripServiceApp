using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TripServiceApp.Models
{
    public class TripActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Activity { get; set; }

        public string Details { get; set; }

        public int TripId { get; set; }

        public int TripUserId { get; set; }

        public Trip Trip { get; set; }

        public long Lat { get; set; }

        public long Lon { get; set; }


        public string Location { get; set; }

        public Boolean IsComplete { get; set; }

        public List<ActivityVote> ActivityVotes { get; set; }

        
    }
}