//using System;
//using System.Configuration;
//using System.Threading.Tasks;
//using Microsoft.Owin;
//using Owin;

//[assembly: OwinStartup(typeof(WebApplicationSignalR_NEW.App_Code.Startup1))]

//namespace WebApplicationSignalR_NEW.App_Code
//{
//    public class Startup1
//    {
//        public void Configuration(IAppBuilder app)
//        {
//            System.Data.SqlClient.SqlDependency.Start(ConfigurationManager.ConnectionStrings["UserConnection"].ConnectionString);
//            app.MapSignalR();
//        }
//    }
//}
