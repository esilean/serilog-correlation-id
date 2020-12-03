using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Serilog.CoId.Api.Called.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalledController : ControllerBase
    {
        private readonly ILogger<CalledController> _logger;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public CalledController(ILogger<CalledController> logger, ICorrelationContextAccessor correlationContextAccessor)
        {
            _logger = logger;
            _correlationContextAccessor = correlationContextAccessor;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var correlationId = _correlationContextAccessor.CorrelationContext.CorrelationId;

            _logger.LogInformation("Log CorrelationId: {CorrelationId}", correlationId);
            return Ok(correlationId);
        }
    }
}
