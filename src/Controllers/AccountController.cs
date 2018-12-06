using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [Authorize]
        [HttpGet("UserProfile")]
        public IActionResult UserProfile()
        {
            return Json(new {
                claims = User.Claims.Select(c => new { c.Type, c.Value}),
                identity = User.Identity.Name
            });
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost("ExternalLogin")]
        public IActionResult ExternalLogin([FromForm]ExternalLoginBindingModel model)
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = model.ReturnUrl ?? "/home/index",
                IsPersistent = true
            }, model.LoginType);
        }
    }
}