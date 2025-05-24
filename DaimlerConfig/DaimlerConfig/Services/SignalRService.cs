using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace DaimlerConfig.Services
{
    public class SignalRService
    {
        private readonly HubConnection connection;

        public SignalRService(HubConnection connection)
        {
            this.connection = connection;
        }

        public async Task StartConnectionAsync()
        {
            try
            {
                if (connection.State == HubConnectionState.Disconnected)
                    await connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR-Startfehler: {ex.Message}");
            }
        }

        public async Task SendMessageToServer(string message)
        {
            if (connection.State != HubConnectionState.Connected)
            {
                await StartConnectionAsync();
            }
            await connection.SendAsync("ReceiveMessage", message);
        }

    

        public void RegisterResponseHandler(Action<string> handler)
        {
            connection.On("ReceiveMessage", handler);
        }

       
    }
}
