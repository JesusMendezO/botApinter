using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters.TwilioWhatsapp;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZokoController : ControllerBase
    {
        private readonly ZokoAdapter _adapter;
        private readonly IBot _bot;

        public ZokoController(ZokoAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
        }

        [HttpPost("bot")]
        [HttpGet("bot")]
        public async Task PostAsync()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            await _adapter.ProcessAsync(Request, Response, _bot, default(CancellationToken));
        }
    }
}
