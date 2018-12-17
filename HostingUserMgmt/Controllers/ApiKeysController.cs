using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HostingUserMgmt.AppServices.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostingUserMgmt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApiKeysController : ControllerBase
    {
        private readonly IApiKeyService apiKeyService;
        public ApiKeysController(IApiKeyService apiKeyService)
        {
            this.apiKeyService = apiKeyService;
        }

        [HttpGet("display")]
        public async Task<IActionResult> GetDisplayApikeys()
        {
            return Ok(await apiKeyService.GetApiKeysForDisplayAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await apiKeyService.GetApiKeyByIdAsync(id));
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateApiKey()
        {
            var newKey = await apiKeyService.CreateApiKey();
            return Ok(newKey);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
