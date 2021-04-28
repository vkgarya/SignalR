using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebApplicationSignalR.App_Code.Startup1))]

namespace WebApplicationSignalR.App_Code
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {

            app.MapSignalR();

        }
    }
}
