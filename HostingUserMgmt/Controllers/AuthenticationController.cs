using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HostingUserMgmt.AppServices.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HostingUserMgmt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IApiKeyService apiKeyService;
        private readonly ILogger<AuthenticationController> logger;
        public AuthenticationController(IApiKeyService apiKeyService, ILogger<AuthenticationController> logger)
        {
            this.apiKeyService = apiKeyService;
            this.logger = logger;
        }

        [HttpGet("verify")]
        public async Task<IActionResult> Verify([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                Response.Headers.Add("WWW-Authenticate", "Basic realm=\"host management\"");
                return Unauthorized();
            }
            var authHeader = AuthenticationHeaderValue.Parse(authorizationHeader);
            try
            {
                if (await apiKeyService.VerifyApiKeyCredentialsAsync(authHeader.Scheme, authHeader.Parameter))
                {
                    return Ok();
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                logger.LogError(default(EventId), ex, $"Error processing basic auth header: {ex.Message}");
                return BadRequest();
            }
        }
    }
}