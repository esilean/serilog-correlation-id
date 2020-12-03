using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Serilog.CoId.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CorrelationIdController : ControllerBase
    {

        private readonly ILogger<CorrelationIdController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public CorrelationIdController(ILogger<CorrelationIdController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogInformation("Called Get method {CorrelationIdController}", nameof(CorrelationIdController));

                var client = _httpClientFactory.CreateClient("CalledClient");

                var response = await client.GetAsync("/called");
                response.EnsureSuccessStatusCode();

                var correlationId = await response.Content.ReadAsStringAsync();
                return Ok(correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
