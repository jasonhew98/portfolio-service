using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Api.Application.SignalR
{
    public class PortfolioHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public async Task SendMessage()
        {
            await Clients.All.SendAsync("ReceiveMessage", "Hello from Server");
        }
    }
}
