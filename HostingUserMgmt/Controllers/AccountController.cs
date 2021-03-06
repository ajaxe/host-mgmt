using System;
using System.Threading.Tasks;
using HostingUserMgmt.AppServices.Abstractions;
using HostingUserMgmt.Domain.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HostingUserMgmt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly ILogger<AccountController> logger;
        public AccountController(IUserService userService,
            ILogger<AccountController> logger)
        {
            this.userService = userService;
            this.logger = logger;
        }
        [Authorize]
        [HttpGet("UserProfile")]
        public async Task<IActionResult> UserProfile()
        {
            try
            {
                var up = await userService.GetUserProfileAsync();
                return Json(up);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
        [HttpPost("Signout")]
        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost("GoogleOAuthLogin")]
        public IActionResult GoogleOAuthLogin([FromForm] ExternalLoginBindingModel model)
        {
            var defaultRedirectUri = Url.Action("Index", "Home");
            logger.LogDebug($"defaultRedirectUri = {defaultRedirectUri}");
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = model.ReturnUrl ?? defaultRedirectUri,
            }, Startup.GoogleOidcAuthScheme);
        }
        [Authorize]
        [HttpDelete("{externalId}")]
        public async Task<IActionResult> DeleteAccount(string externalId)
        {
            try
            {
                await userService.DeleteUserByExternalIdAsync(externalId);
                return Ok();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }
    }
}