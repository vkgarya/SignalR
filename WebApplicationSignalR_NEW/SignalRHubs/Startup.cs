using Microsoft.Owin;
using Owin;
using System.Configuration;

[assembly: OwinStartup(typeof(WebApplicationSignalR_NEW.SignalRHubs.Startup))]

namespace WebApplicationSignalR_NEW.SignalRHubs
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            
            // Vijay::Step - 4
            app.MapSignalR();
        }
    }
}
