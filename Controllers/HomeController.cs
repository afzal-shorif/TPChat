using CimpleChat.Models;
using CimpleChat.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using TPChat.Services;

namespace TPChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpContext _context;
        private readonly IDataProtector _protector;
        private readonly IConfiguration _configuration;
        private readonly IGroupMessageService _channelService;
        

        public HomeController(IHttpContextAccessor httpContextAccessor,
                              IDataProtectionProvider protectionProvider,
                              IConfiguration configuration,
                              IGroupMessageService groupMessageService) {
            
            _context = httpContextAccessor?.HttpContext;
            _protector = protectionProvider.CreateProtector(configuration["CookiePurpose"].ToString());
            _configuration = configuration;
            _channelService = groupMessageService;
        }
        
        public IActionResult Index()
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

            ViewBag.Channels = _channelService.GetChannelList();
            return View();
        } 
    }
}
