using Microsoft.AspNet.SignalR;

namespace WebApplicationSignalR_NEW.SignalRHubs
{
    // Vijay::Step - 5
    public class UserHub : Hub
    {
        public static void Show()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<UserHub>();
            context.Clients.All.displayUsers();
        }
    }
}