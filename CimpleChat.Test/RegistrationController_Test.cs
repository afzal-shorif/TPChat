
using CimpleChat.Controllers;
using CimpleChat.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using CimpleChat.Test.Infrastructure;
using CimpleChat.Services.UserService;

namespace CimpleChat.Test
{
    #region Mock Objects
    public class UserService : IUserService
    {
        List<string> userList = new List<string>();

        public User AddNewUser(string userName)
        {
            userList.Add(userName);
            return new User();
        }

        public User GetUser(long userId)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetUsers()
        {
            throw new NotImplementedException();
        }

        public bool IsUsernameAvailable(string username)
        {
            if (userList.Contains(username))
            {
                return false;
            }

            userList.Add(username);
            return true;
        }
    }

    #endregion

    public class RegistrationController_Test
    {
        #region Fields

        private readonly IConfiguration configuration;
        private readonly IDataProtectionProvider protectionProvider;
        private readonly IUserService userService;
        //private readonly IGetNextId getNextId;
        private RegistrationController registrationController;

        #endregion

        #region Ctor

        public RegistrationController_Test()
        {

            var configurations = new Dictionary<string, string>()
            {
                {"CookiePurpose", "TestString" }
            };


            configuration = new ConfigurationBuilder().AddInMemoryCollection(configurations).Build();
            protectionProvider = Helper.GetRequiredService<IDataProtectionProvider>();
            //getNextId = Helper.GetRequiredService<IGetNextId>();
            userService = new UserService();          
        }

        #endregion

        #region Testcase
        [Fact]
        public void InvalidUsernameTest_1()
        {
            registrationController = new RegistrationController(configuration, protectionProvider, userService);

            // null
            var responseResult = registrationController.Index(null) as JsonResult;

            var response = responseResult.Value;

            Assert.NotNull(responseResult);
            Assert.NotNull(GetProperty(response, "status"));
            Assert.Equal("error", GetPropertyValue(response, "status"));
            Assert.Null(GetProperty(response, "cookieInfo"));
            Assert.NotNull(GetProperty(response, "message"));
        }

        [Fact]
        public void InvalidUsernameTest_2()
        {
            // Test registration with empty object
            registrationController = new RegistrationController(configuration, protectionProvider, userService);

            var user = new RegisterUser();

            var responseResult = registrationController.Index(user) as JsonResult;
            var response = responseResult.Value;

            
            Assert.NotNull(responseResult);                                                                // Response should not be null

            Assert.NotNull(GetProperty(response, "status"));                                  // Response must have status property
            Assert.Equal("error", GetPropertyValue(response, "status"));

            Assert.Null(GetProperty(response, "cookieInfo"));                                 // Cookie property should be null present for invalid user

            Assert.NotNull(GetProperty(response, "message"));                                 // Response should have a message property for invaid user
        }

        [Fact]
        public void InvalidUsernameTest_3()
        {
            registrationController = new RegistrationController(configuration, protectionProvider, userService);
            var user = new RegisterUser()
            {
                UserName = "1Test",
                Gender = 0,
                Age = 17
            };

            var responseResult = registrationController.Index(user) as JsonResult;
            var response = responseResult.Value;

            Assert.NotNull(responseResult);
            Assert.NotNull(GetProperty(response, "status"));
            Assert.NotNull(GetProperty(response, "message"));

            Assert.Null(GetProperty(response, "cookieInfo"));
            
            Assert.Equal("error", GetPropertyValue(response, "status"));
        }

        [Fact]
        public void InvalidUsernameTest_4()
        {
            registrationController = new RegistrationController(configuration, protectionProvider, userService);
            var invalid2 = new RegisterUser()
            {
                UserName = "Test#",
                Gender = 0,
                Age = 17
            };

            var responseResult = registrationController.Index(invalid2) as JsonResult;
            var response = responseResult.Value;

            Assert.NotNull(responseResult);
            Assert.NotNull(GetProperty(response, "status"));
            Assert.Equal("error", GetPropertyValue(response, "status"));
            Assert.Null(GetProperty(response, "cookieInfo"));
            Assert.NotNull(GetProperty(response, "message"));
        }

        [Fact]
        public void InvalidUsernameTest_5()
        {
            registrationController = new RegistrationController(configuration, protectionProvider, userService);
            var invalid3 = new RegisterUser()
            {
                UserName = "tes@t",
                Gender = 0,
                Age = 17
            };

            var responseResult = registrationController.Index(invalid3) as JsonResult;
            var response = responseResult.Value;

            Assert.NotNull(responseResult);
            Assert.NotNull(GetProperty(response, "status"));
            Assert.Equal("error", GetPropertyValue(response, "status"));
            Assert.Null(GetProperty(response, "cookieInfo"));
            Assert.NotNull(GetProperty(response, "message"));
        }

        [Fact]
        public void InvalidUsernameTest_6()
        {
            registrationController = new RegistrationController(configuration, protectionProvider, userService);
            var invalid4 = new RegisterUser()
            {
                UserName = "Te",
                Gender = 0,
                Age = 17
            };

            var responseResult = registrationController.Index(invalid4) as JsonResult;
            var response = responseResult.Value;

            Assert.NotNull(responseResult);
            Assert.NotNull(GetProperty(response, "status"));
            Assert.Equal("error", GetPropertyValue(response, "status"));
            Assert.Null(GetProperty(response, "cookieInfo"));
            Assert.NotNull(GetProperty(response, "message"));
        }

        [Fact]
        public void InvalidUsernameTest_7()
        {
            registrationController = new RegistrationController(configuration, protectionProvider, userService);
            var invalid5 = new RegisterUser()
            {
                UserName = "TesttheResultWithLongStringLetSeeWhathappend",
                Gender = 0,
                Age = 17
            };

            var responseResult = registrationController.Index(invalid5) as JsonResult;
            var response = responseResult.Value;

            Assert.NotNull(responseResult);
            Assert.NotNull(GetProperty(response, "status"));
            Assert.Equal("error", GetPropertyValue(response, "status"));
            Assert.Null(GetProperty(response, "cookieInfo"));
            Assert.NotNull(GetProperty(response, "message"));
        }

        [Fact]
        public void InvalidUsernameTest_8()
        {
            registrationController = new RegistrationController(configuration, protectionProvider, userService);
            var invalid6 = new RegisterUser()
            {
                UserName = "Testthe Result",
                Gender = 0,
                Age = 17
            };

            var responseResult = registrationController.Index(invalid6) as JsonResult;
            var response = responseResult.Value;

            Assert.NotNull(response);
            Assert.NotNull(GetProperty(response, "status"));
            Assert.Equal("error", GetPropertyValue(response, "status"));
            Assert.Null(GetProperty(response, "cookieInfo"));
            Assert.NotNull(GetProperty(response, "message"));
        }

        [Fact]
        public void InvalidAgeTest()
        {
            registrationController = new RegistrationController(configuration, protectionProvider, userService);
            var user = new RegisterUser()
            {
                UserName = "TesttheResult",
                Gender = 0,
                Age = 10
            };

            var responseResult = registrationController.Index(user) as JsonResult;
            var response = responseResult.Value;

            Assert.NotNull(responseResult);
            Assert.NotNull(GetProperty(response, "status"));

            Assert.Equal("error", GetPropertyValue(response, "status"));
            Assert.Null(GetProperty(response, "cookieInfo"));

            Assert.Null(GetProperty(response, "cookieInfo"));
            Assert.NotNull(GetProperty(response, "message"));
        }

        [Fact]
        public void ValidUserTest()
        {
            registrationController = new RegistrationController(configuration, protectionProvider, userService);
            var user = new RegisterUser()
            {
                UserName = "TesttheResult",
                Gender = 0,
                Age = 16
            };

            var responseResult = registrationController.Index(user) as JsonResult;
            var response = responseResult.Value;

            Assert.NotNull(responseResult);
            Assert.NotNull(GetProperty(response, "status"));
            Assert.Equal("success", GetPropertyValue(response, "status"));
            Assert.NotNull(GetProperty(response, "cookieInfo"));
            Assert.NotEqual("", GetPropertyValue(response, "cookieInfo"));
        }

        [Fact]
        public void UniqueUsernameValidation()
        {
            registrationController = new RegistrationController(configuration, protectionProvider, userService);
            var user = new RegisterUser()
            {
                UserName = "TestName",
                Gender = 0,
                Age = 18
            };

            registrationController.Index(user);

            var responseResult = registrationController.IsUserNameAvailable("TestName") as JsonResult;
            var response = responseResult.Value;

            Assert.NotNull(responseResult);
            Assert.NotNull(GetProperty(response, "status"));
            Assert.Equal("error", GetPropertyValue(response, "status"));
            Assert.Equal("Username \"TestName\" is not available.", GetPropertyValue(response, "message"));
        }

        [Fact]
        public void UniqueUsernameDuringRegistrationTest()
        {
            registrationController = new RegistrationController(configuration, protectionProvider, userService);
            var user = new RegisterUser()
            {
                UserName = "TesttheResult",
                Gender = 0,
                Age = 17
            };

            registrationController.Index(user);
            
            var responseResult = registrationController.Index(user) as JsonResult;
            var response = responseResult.Value;

            Assert.NotNull(responseResult);

            Assert.NotNull(GetProperty(response, "status"));
            Assert.Equal("error", GetPropertyValue(response, "status"));

            Assert.Null(GetProperty(response, "cookieInfo"));
        }

        #endregion

        #region Private Methods

        private PropertyInfo GetProperty(object obj, string name)
        {
            return obj.GetType().GetProperty(name);
        }

        private object GetPropertyValue(object obj, string name)
        {
            return obj.GetType().GetProperty(name).GetValue(obj);;
        }

        #endregion

    }
}
