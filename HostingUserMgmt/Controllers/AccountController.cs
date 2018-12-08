using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HostingUserMgmt.Helpers.Authentication;
using HostingUserMgmt.AppServices.Abstractions;
using HostingUserMgmt.Domain.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostingUserMgmt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        public AccountController(IUserService userService)
        {
            this.userService = userService;
        }
        [Authorize]
        [HttpGet("UserProfile")]
        public async Task<IActionResult> UserProfile()
        {
            try
            {
                var up = await userService.GetUserProfile();
                return Json(up);
            }
            catch(InvalidOperationException ioe)
            {
                return NotFound();
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("Signout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost("ExternalLogin")]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin([FromForm]ExternalLoginBindingModel model)
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = model.ReturnUrl ?? "/home/index",
                IsPersistent = true
            }, model.LoginType);
        }
        [HttpDelete("{externalId}")]
        public async Task<IActionResult> DeleteAccount(string externalId)
        {
            try
            {
                await userService.DeleteUserByExternalIdAsync(externalId);
                return Ok();
            }
            catch(InvalidOperationException)
            {
                return BadRequest();
            }
        }
    }
}