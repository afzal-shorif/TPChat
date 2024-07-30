using CimpleChat.Models;
using CimpleChat.Services.UserService;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CimpleChat.Controllers
{
    public class RegistrationController : Controller
    {
        #region Properties

        private readonly IDataProtector _protector;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        #endregion

        #region Ctor

        public RegistrationController(
            IConfiguration configuration,
            IDataProtectionProvider protectionProvider, 
            IUserService userService)
        {
            _protector = protectionProvider.CreateProtector(configuration["CookiePurpose"]);
            _userService = userService;
            _configuration = configuration;
        }

        #endregion

        #region Public Methods

        [HttpPost]
        public IActionResult Index([FromBody] RegisterUser userInfo)
        {
            string validationResponse = ValidateUser(userInfo);

            if (!string.IsNullOrEmpty(validationResponse))
            {
                var resp = new
                {
                    status = "error",
                    message = validationResponse,
                };

                return Json(resp);
            }

            if (_userService.IsUsernameAvailable(userInfo.UserName))
            {
                var user = _userService.AddNewUser(userInfo.UserName);

                string cookeiInfo = System.Text.Json.JsonSerializer.Serialize(user);

                var resp = new
                {
                    status = "success",
                    message = $"Username \"{userInfo.UserName}\" is available.",
                    userId = user.Id,
                    username = user.Name,
                    cookieInfo = _protector.Protect(cookeiInfo),
                };
                return Json(resp);
            }

            var response = new
            {
                status = "error",
                message = $"Username \"{userInfo.UserName}\" is not available."
            };

            return Json(response);
        }  

        public IActionResult IsUserNameAvailable(string username)
        {
            Regex regex = new Regex(@"^[a-zA-Z][a-zA-Z0-9]{2,17}$");
            string status = string.Empty;
            string message = string.Empty;

            if (string.IsNullOrEmpty(username) || !regex.IsMatch(username))
            {
                status = "error";
                message = $"Username \"{username}\" is invalid.";
            }
            else
            {
                if (!_userService.IsUsernameAvailable(username))
                {
                    status = "error";
                    message = $"Username \"{username}\" is not available.";
                }
                else
                {
                    status = "success";
                    message = $"Username \"{username}\" is available.";
                }
            }
            
            var response = new
            {
                status = status,
                message = message,
            };

            return Json(response);
        }

        #endregion

        #region Private Methods

        private string ValidateUser(RegisterUser userInfo)
        {
            Regex regex = new Regex(@"^[a-zA-Z][a-zA-Z0-9]{2,17}$");

            if (userInfo == null)
            {
                return "User info is invalid.";
            }            
            else if (!string.IsNullOrEmpty(userInfo.UserName) && !regex.IsMatch(userInfo.UserName))
            {
                return "Invalid username.";
            }
            else if (userInfo.Age < 16 || userInfo.Age > 100)
            {
                return "Invalid age.";
            }

            return "";
        }

        #endregion
    }
}
