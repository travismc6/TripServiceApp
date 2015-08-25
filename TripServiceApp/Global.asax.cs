using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using TripServiceApp.Models;

namespace TripServiceApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            System.Data.Entity.Database.SetInitializer(
                new TripServiceAppContextInitializer());

            GlobalConfiguration.Configure(WebApiConfig.Register);

    
        }
    }
}
