using CimpleChat.Models;
using CimpleChat.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace CimpleChat.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IDataProtector _protector;
        private readonly IUserService _userService;
        
        public RegistrationController(IDataProtectionProvider protectionProvider, IUserService userService)
        {
            _protector = protectionProvider.CreateProtector("TestString");
            _userService = userService;
        }
        
        [HttpPost]
        public IActionResult Index([FromBody] RegisterUser userInfo)
        {
            if (userInfo == null)
            {
                var resp = new
                {
                    status = "error",
                    message = "Invalid User Info.",
                };

                return Json(resp);
            }

            var user = _userService.AddNewUser(userInfo.UserName);

            string cookeiInfo = System.Text.Json.JsonSerializer.Serialize(user);

            var response = new
            {
                status = "success",
                cookieInfo = _protector.Protect(cookeiInfo),
            };

            return Json(response);
        }
    }
}
