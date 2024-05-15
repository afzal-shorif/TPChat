using CimpleChat.Models;
using CimpleChat.Services.ChannelService;
using CimpleChat.Services.SocketService;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace CimpleChat.Controllers
{
    public class ChannelController : Controller
    {
        private readonly IGroupMessageService _channelService;
        private readonly HttpContext? _context;
        private readonly IDataProtector _protector;
        private readonly IConfiguration _configuration;

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
            string protectedCookie = _context!.Request.Cookies["userInfo"] ?? string.Empty;

            if (!string.IsNullOrEmpty(protectedCookie))
            {
                string cookieValue = _protector.Unprotect(protectedCookie);
                var user = System.Text.Json.JsonSerializer.Deserialize<User>(cookieValue);
                
                if(user != null)
                {
                    ViewBag.Username = user.Name;
                    ViewBag.UserId = user.Id;
                    ViewBag.UserProfileCharacter = user.Name.Substring(0, 1).ToUpper();

                    // add user to the channel
                    _channelService.AddUserToChannel(channelId, user.Id);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ChannelId = channelId;

            return View();
        }

        public IActionResult GetChannelList()
        {
            var channels = _channelService.GetChannelList();
            
            return Json(channels);
        }
    }
}
