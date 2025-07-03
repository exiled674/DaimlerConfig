
using Microsoft.AspNetCore.SignalR;
namespace DConfigServer.Hubs
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

      

        
    }
}
