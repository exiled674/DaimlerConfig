using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using DaimlerConfigServer.Hubs;

namespace DaimlerConfigServer.Controllers
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
