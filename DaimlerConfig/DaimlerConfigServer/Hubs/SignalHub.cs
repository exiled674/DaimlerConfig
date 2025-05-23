
using Microsoft.AspNetCore.SignalR;
namespace DaimlerConfigServer.Hubs
{
    public class SignalHub : Hub
    {

        public async Task ReceiveMessage(string message)
        {
            await BroadCastMessage(message);
        }

        public async Task BroadCastMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task LockMessage(int id)
        {
            await Clients.Others.SendAsync("LockMessage", id);
        }

        
    }
}
