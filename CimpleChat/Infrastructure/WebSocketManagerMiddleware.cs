using CimpleChat.Models;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json.Serialization;
using System.Net;
using CimpleChat.Services.SocketService;

namespace CimpleChat.Infrastructure
{
    public class WebSocketManagerMiddleware
    {
        private readonly WebSocketHandler _channelMessageHandler;
        private readonly IConfiguration _configuration;
        private readonly IDataProtector _protector;
        private readonly RequestDelegate _next;
        public WebSocketManagerMiddleware(WebSocketHandler channelMessageHandler,
                                IConfiguration configuration,
                                IDataProtectionProvider protectorProvider,
                                RequestDelegate next)
        {
            _channelMessageHandler = channelMessageHandler;
            _configuration = configuration;
            _protector = protectorProvider.CreateProtector(_configuration["CookiePurpose"]);
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = (int) HttpStatusCode.BadGateway;
                return;
            }
            
            // read cookie to get user info
            string protectedCookie = context.Request.Cookies["userInfo"];

            if (string.IsNullOrEmpty(protectedCookie))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
            
            string cookie = _protector.Unprotect(protectedCookie);

            var user = System.Text.Json.JsonSerializer.Deserialize<User>(cookie);

            if (user == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            string sChannelId = context.Request.Query["channelId"].ToString();
            Int32.TryParse(sChannelId, out int channelId);

            if(channelId <= 0)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var ws = await context.WebSockets.AcceptWebSocketAsync();
            await _channelMessageHandler.OnConnectAsync(ws, channelId, user);
            await _channelMessageHandler.ReceiveAsync(ws, channelId, user);
        }
    }
}
