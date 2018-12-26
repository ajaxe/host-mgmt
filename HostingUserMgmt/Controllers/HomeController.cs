using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HostingUserMgmt.Helpers.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HostingUserMgmt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly GoogleConfig googleOptions;
        public HomeController(ILogger<HomeController> logger,
            IOptions<GoogleConfig> options)
        {
            this.logger = logger;
            this.googleOptions = options?.Value ?? throw new ArgumentNullException("googleOptions");
        }
        public IActionResult Index()
        {
            ViewBag.ClientId = googleOptions.ClientId;
            return View();
        }
    }
}