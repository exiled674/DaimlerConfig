using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using DConfigServer.Hubs;
using System.Runtime.CompilerServices;

namespace DConfigServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(IHubContext<SignalHub> hubcontext) : ControllerBase
    {
        [HttpPost("notify")]
        public async Task<IActionResult> NotifyClients([FromBody] string message)
        {
            await hubcontext.Clients.All.SendAsync("ReceiveMessage", message);
            return Ok(message);
        }
    }
}

