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
            _context.Response.Cookies.Append("ChannelId", channelId.ToString());

            return View();
        }

        public async Task ConnectToChannel(int channelId)
        {
            if (!_context.WebSockets.IsWebSocketRequest)
            {
                _context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            }

            // check the user is available in that channle

            
            var ws = await _context.WebSockets.AcceptWebSocketAsync();

            // Add this connection to the websocket list
            _channelService.AddNewConnection(channelId, ws);

            string protectedCookie = _context.Request.Cookies["userInfo"];

            User user = null;

            if (!string.IsNullOrEmpty(protectedCookie))
            {
                string cookieValue = _protector.Unprotect(protectedCookie);
                user = System.Text.Json.JsonSerializer.Deserialize<User>(cookieValue);
            }

            await ReceiveMessage(ws, channelId, user.Id);
        }

        private async Task ReceiveMessage(WebSocket ws, int channelId, int userId)
        {
            //Int32.TryParse(_configuration["BufferSize"], out int bufferSize);

            var announcedResponse = await _channelService.AddNewAnnounceMessage(channelId, userId);
            //await BroadcastMessageHistory(ws, channelId);


            await BroadcastMessage(channelId, System.Text.Json.JsonSerializer.Serialize(announcedResponse));

            byte[] byteArray = new byte[1024 * 4];
            
            while(ws.State == WebSocketState.Open)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(byteArray);

                var webSocketResult = await ws.ReceiveAsync(buffer, CancellationToken.None);
                
                if(webSocketResult.MessageType == WebSocketMessageType.Text)
                {
                    string msgString = Encoding.UTF8.GetString(byteArray, 0, webSocketResult.Count);
                    if(msgString != null && msgString != "_PING_")
                    {
                        var msgResponse = await _channelService.AddNewMessage(channelId, userId, msgString);
                        
                        await BroadcastMessage(channelId, System.Text.Json.JsonSerializer.Serialize(msgResponse));
                    }
                }
            }

            // remove the connection
        }

        private async Task BroadcastMessage(int channelId, string message)
        {
            IList<WebSocket> connections = _channelService.GetConnections(channelId);

            ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));

            foreach(WebSocket ws in connections)
            {
                if (ws.State == WebSocketState.Open)
                {
                    await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else if(ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted)
                {
                    //_channelService.RemoveConnection(ws);
                    //ws.CloseAsync();
                }
            }
        }

        private async Task BroadcastMessageHistory(WebSocket ws, int channelId)
        {
            IList<Message> messages = _channelService.GetMessages(channelId);
            string msgResponse = System.Text.Json.JsonSerializer.Serialize(messages);

            ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msgResponse));
            
            if (ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted)
            {
                //_channelService.RemoveConnection(ws);
                //ws.CloseAsync();
            }            
        }
    }
}
