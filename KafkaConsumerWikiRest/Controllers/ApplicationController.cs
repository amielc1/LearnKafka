using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace KafkaConsumerWikiRest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : ControllerBase
    {
        public ApplicationController()
        {
            Log.Information("ApplicationController Started");
        }

        [HttpPost("admin/close")]
        public IActionResult CloseApplication([FromServices] IHostApplicationLifetime applicationLifetime)
        {
            Log.Information("Shutdown initiated.");
            applicationLifetime.StopApplication();
            return Ok("Shutdown initiated successfully.");
        }
    }
}
