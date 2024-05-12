using CimpleChat.Models;
using CimpleChat.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;

namespace CimpleChat.Controllers
{
    public class ChannelController : Controller
    {
        private readonly IGroupMessageService _channelService;
        private readonly HttpContext _context;
        private readonly IDataProtector _protector;
        private readonly IConfiguration _configuration;
        private readonly ChannelMessageHandler _channelMessageHandlerl;

        public ChannelController(IGroupMessageService channelService, 
            IHttpContextAccessor context,
            IDataProtectionProvider dataProtectionProvider,
            IConfiguration configuration)
        {
            _channelService = channelService;
            _context = context.HttpContext;
            _protector = dataProtectionProvider.CreateProtector("TestString");
            _configuration = configuration;
        }

        public IActionResult Index(int channelId)
        {
            string protectedCookie = _context.Request.Cookies["userInfo"];

            if (!string.IsNullOrEmpty(protectedCookie))
            {
                string cookieValue = _protector.Unprotect(protectedCookie);
                var user = System.Text.Json.JsonSerializer.Deserialize<User>(cookieValue);

                ViewBag.Username = user.Name;
                ViewBag.UserId = user.Id;
                ViewBag.UserProfileCharacter = user.Name.Substring(0, 1).ToUpper();

                // add user to the channel
                _channelService.AddUserToChannel(channelId, user.Id);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ChannelId = channelId;

            return View();
        }
    }
}
