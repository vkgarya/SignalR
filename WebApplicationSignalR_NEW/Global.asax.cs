using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace WebApplicationSignalR_NEW
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // Vijay::Step - 2
            //SqlDependency.Stop(ConfigurationManager.ConnectionStrings["UserConnection"].ConnectionString);
            //SqlDependency.Start(ConfigurationManager.ConnectionStrings["UserConnection"].ConnectionString);
            
            SqlDependency.Start(ConfigurationManager.ConnectionStrings["UserConnection"].ConnectionString);

        }

        // Vijay::Step - 3
        protected void Application_End()
        {
            SqlDependency.Stop(ConfigurationManager.ConnectionStrings["CustomerConnection"].ConnectionString);
        }
    }
}