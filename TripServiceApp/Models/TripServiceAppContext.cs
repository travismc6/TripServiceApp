using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TripServiceApp.Models
{
    public class TripServiceAppContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public TripServiceAppContext() : base("name=TripServiceAppContext")
        {
        }

        public DbSet<Trip> Trips { get; set; }

        public System.Data.Entity.DbSet<TripServiceApp.Models.TripUser> TripUsers { get; set; }

        public System.Data.Entity.DbSet<TripServiceApp.Models.TripComment> TripComments { get; set; }

        public System.Data.Entity.DbSet<TripServiceApp.Models.TripActivity> TripActivities { get; set; }

        public System.Data.Entity.DbSet<TripServiceApp.Models.ActivityVote> ActivityVotes { get; set; }

        public System.Data.Entity.DbSet<TripServiceApp.Models.PushRegistrations> PushRegistrations { get; set; }

        public System.Data.Entity.DbSet<TripServiceApp.Models.TripPicture> TripPictures { get; set; }
    
    }
}
