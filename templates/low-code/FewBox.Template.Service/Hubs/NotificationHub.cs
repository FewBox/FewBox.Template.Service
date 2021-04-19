using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FewBox.Template.Service.Hubs
{
    [Authorize(Policy="JWTRole_Hub")]
    public class NotificationHub : Hub
    {
        public async Task Notify(string message, string description)
        {
            await Clients.All.SendAsync("notify", message, description);
        }
    }
}
