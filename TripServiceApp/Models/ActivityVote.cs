using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TripServiceApp.Models
{
    public class ActivityVote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TripUserId { get; set; }
        public int TripActivityId { get; set; }

        public TripUser TripUser { get; set; }

        public int Vote { get; set; }

        public TripActivity TripActivity { get; set; }
    }
}