using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TripServiceApp.Models
{
    public class TripPicture
    {
        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int TripUserId { get; set; }

        public string Description { get; set; }

        public double Lat { get; set; }

        public double Lon { get; set; }

        public string PictureUrl { get; set; }

        public string LocationName { get; set; }

        public TripUser TripUser { get; set; }

    }
}