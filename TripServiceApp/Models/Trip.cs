using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TripServiceApp.Models
{
    public class Trip
    {
        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string Code { get; set; }

        [Required]
        public string Destination { get; set; }

        public string Description { get; set; }

        [Required]
        public string MyName { get; set; }

        public virtual ICollection<TripUser> Users { get; set; }
        
        public string UserJson { get; set; }

        public TripStatus Status { get; set; }

    }
}