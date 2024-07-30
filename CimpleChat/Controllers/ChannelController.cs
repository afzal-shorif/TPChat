using CimpleChat.Models;
using CimpleChat.Models.Channel;
using CimpleChat.Services.ChannelService;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

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

        public IActionResult Create(string channelName, int type)
        {
            string protectedCookie = _context!.Request.Cookies["userInfo"] ?? string.Empty;
            
            if (!string.IsNullOrEmpty(protectedCookie))
            {
                string cookieValue = _protector.Unprotect(protectedCookie);
                var user = System.Text.Json.JsonSerializer.Deserialize<User>(cookieValue);

                if (user != null)
                {
                    string validateMsg = ValidateChannelName(channelName);

                    if (string.IsNullOrEmpty(validateMsg) && (type == 0 || type == 1))
                    {
                        long channelId = _channelService.AddNewChannel(channelName, (type == 0) ? ChannelType.@private : ChannelType.@public);

                        _channelService.AddUserToChannel(channelId, user.Id);

                        return Json(new { ChannelId = channelId });
                    }
                    else
                    {
                        return Json(new { Error = validateMsg });
                    }                  
                }
            }

            return Json(new { Error = "Invalid user info." });
        }

        public void AddMemberToChannel(long channelId, long userId)
        {
            string protectedCookie = _context!.Request.Cookies["userInfo"] ?? string.Empty;

            if (!string.IsNullOrEmpty(protectedCookie))
            {
                string cookieValue = _protector.Unprotect(protectedCookie);
                var user = System.Text.Json.JsonSerializer.Deserialize<User>(cookieValue);

                if (user != null)
                {
                    if(userId == user.Id)
                    {
                        _channelService.AddMemberToPublicChannel(channelId, user.Id);
                    }
                    else
                    {
                        _channelService.AddMemberToPrivateChannel(channelId, userId, user.Id);
                    }
                }
            }
        }

        public IActionResult SearchChannels(string search)
        {            
            var channelList = _channelService.SearchChannel(search.Trim());

            return Json(channelList);
        }

        private string ValidateChannelName(string channelName)
        {
            Regex regex = new Regex(@"^[a-zA-Z][a-zA-Z0-9]{2,17}$");

            if (!string.IsNullOrEmpty(channelName) && !regex.IsMatch(channelName))
            {
                return "Invalid channel name.";
            }
            else if (channelName.Length <=2 )
            {
                return "Invalid channel name.";
            }
            else if(!IsChannelNameAvailable(channelName))
            {
                return "Channel name is already available.";
            }

            return "";
        }

        private bool IsChannelNameAvailable(string name)
        {
            // determine that is channel name is available to register.
            // return false if a channel is already exist under same name.

            if (_channelService.IsChannelNameExist(name))
            {
                return false;
            }
            return true;
        }
    }
}
