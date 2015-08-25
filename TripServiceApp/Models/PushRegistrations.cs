using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TripServiceApp.Models
{
    public class PushRegistrations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string RegistrationId { get; set; }

        public int TripUserId { get; set; }

        public TripUser TripUser { get; set; }
    }
}