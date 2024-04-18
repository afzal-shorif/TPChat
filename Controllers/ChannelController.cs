using CimpleChat.Models;
using CimpleChat.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CimpleChat.Controllers
{
    public class ChannelController : Controller
    {
        private readonly IGroupMessageService _channelService;
        private readonly HttpContext _context;
        private readonly IDataProtector _protector;

        public ChannelController(IGroupMessageService channelService, 
            IHttpContextAccessor context,
            IDataProtectionProvider dataProtectionProvider)
        {
            _channelService = channelService;
            _context = context.HttpContext;
            _protector = dataProtectionProvider.CreateProtector("TestString");
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
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            _context.Response.Cookies.Append("ChannelId", channelId.ToString());

            return View();
        }
    }
}
