using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TripServiceApp.Models
{
    public class TripServiceAppContextInitializer : DropCreateDatabaseIfModelChanges<TripServiceAppContext>
    {
        protected override void Seed(TripServiceAppContext context)
        {
            base.Seed(context);

         

            //var Trips = new List<Trip>
            //{
            //    new Trip{Destination = "St. Louis"},
            //    new Trip{Destination = "Houston"}
            //};

            //Trips.ForEach(t => context.Trips.Add(t));
            //context.SaveChanges();

        }
    }
}