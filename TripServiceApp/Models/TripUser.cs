using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TripServiceApp.Models
{
  
    public class TripUser
    {
        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public int? TripId { get; set; }

        public Trip Trip { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public bool IsCreator { get; set; }

        public string TripCode { get; set; }

        public TripUserStatus TripStatus { get; set; }

        public double Lat { get; set; }

        public double Lon { get; set; }

        public List<TripComment> TripComments { get; set; }
    }
}