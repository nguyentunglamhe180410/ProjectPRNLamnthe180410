using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ProjectPRNLamnthe180410.Hubs
{
    public class LightNovelHub : Hub
    {
        public async Task SendUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveUpdate", message);
        }

    }
}
