using CimpleChat.Models;
using CimpleChat.Models.Chat;
using CimpleChat.Services.ChannelService;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CimpleChat.Services.SocketService
{
    public class ChannelMessageHandler : WebSocketHandler
    {
        private readonly IGroupMessageService _channelService;

        public ChannelMessageHandler(IGroupMessageService groupMessageService) : base()
        {
            _channelService = groupMessageService;
        }

        public override async Task OnConnectAsync(WebSocket ws, int channelId, User user)
        {
            // add web socket connection to the channel
            _channelService.AddNewConnection(channelId, ws);

            // send last 10 message history to the new user
            var messages = _channelService.GetMessages(channelId);
            if(messages.Count > 0)
            {
                var messageResponse = new WebSocketResponse<IList<MessageResponse>>("Message", messages);

                await SendMessageAsync(ws, JsonSerializer.Serialize(messageResponse));
            }

            // save and send join (announce) message
            var connectionList = _channelService.GetConnections(channelId);

            var announced = await _channelService.AddNewAnnounceMessage(channelId, user.Id, "join");
            var announceResponse = new WebSocketResponse<MessageResponse>("Announce", announced);

            await SendMessageAsync(connectionList, JsonSerializer.Serialize(announceResponse));

            // send updated user list
            var activeUserList = _channelService.GetActiveUsers(channelId);
            var userListResponse = new WebSocketResponse<IList<ActiveUserResponse>>("ActiveChannelUsers", activeUserList);

            await SendMessageAsync(connectionList, JsonSerializer.Serialize(userListResponse));
        }

        public override async Task OnDisconnectAsync(WebSocket ws, int channelId, User user)
        {
            // remove connection and user from channel
            _channelService.RemoveConnection(channelId, ws);
            _channelService.RemoveUser(channelId, user.Id);

            // send leave message
            var connectionList = _channelService.GetConnections(channelId);
            var announce = await _channelService.AddNewAnnounceMessage(channelId, user.Id, "leave");
            var announceResponse = new WebSocketResponse<MessageResponse>("Message", announce);

            await SendMessageAsync(connectionList, JsonSerializer.Serialize(announceResponse));

            // send updated user list
            var activeUserList = _channelService.GetActiveUsers(channelId);
            var userListResponse = new WebSocketResponse<IList<ActiveUserResponse>>("ActiveChannelUsers", activeUserList);

            await SendMessageAsync(connectionList, JsonSerializer.Serialize(userListResponse));

        }

        public override async Task ReceiveAsync(WebSocket ws, int channelId, User user)
        {
            byte[] byteArray = new byte[1024];
            bool isCloseMessageReceived = false;
            
            while (ws.State == WebSocketState.Open)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(byteArray);
                var response = await ws.ReceiveAsync(buffer, CancellationToken.None);

                if (response.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(byteArray, 0, response.Count);

                    await ProcessClientMessage(ws, channelId, user, message);
                }
                else if (response.MessageType == WebSocketMessageType.Close)
                {
                    isCloseMessageReceived = true;
                    await OnDisconnectAsync(ws, channelId, user);
                }
            }

            if (!isCloseMessageReceived)
            {
                ws.Abort();               
                await OnDisconnectAsync(ws, channelId, user);               
            }
        }


        public async ValueTask ProcessClientMessage(WebSocket ws, int channelId, User user, string clientResponse)
        {
            var option = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = false,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };

            var responseObject = JsonSerializer.Deserialize<WebSocketResponse<object>>(clientResponse, option);

            switch (responseObject?.Type)
            {
                case "Message":
                    {                       
                        MessageRequest? msgInfo = (JsonSerializer.Deserialize<WebSocketResponse<MessageRequest>>(clientResponse, option))?.Data;

                        if (msgInfo == null)
                        {
                            return;
                        }

                        msgInfo.ChannelId = channelId;
                        msgInfo.UserId = user.Id;

                        var result = await _channelService.AddNewMessage(msgInfo);

                        // send msg saved status
                        var msgStatus = new Models.Chat.MessageStatus()
                        {
                            MessageId = result.MessageId,
                            TempMessageId = msgInfo.TempMessageId,
                            Status = result.Status,
                        };

                        var msgStatusResponse = new WebSocketResponse<Models.Chat.MessageStatus>("MessageStatus", msgStatus);

                        await SendMessageAsync(ws, JsonSerializer.Serialize(msgStatusResponse));

                        // send msg to the other user
                        var socketResponse = new WebSocketResponse<MessageResponse>("Message", result);

                        var connections = _channelService.GetConnections(channelId);
                        
                        // remove the user that send the message
                        await SendMessageAsync(connections.Where(c => c != ws).ToList(), JsonSerializer.Serialize(socketResponse));                                         
                    }
                    break;
                case "MessageStatus":
                    {
                        Models.Chat.MessageStatus? messageStatus = (JsonSerializer.Deserialize<WebSocketResponse<Models.Chat.MessageStatus>>(clientResponse, option))?.Data;

                        if (messageStatus == null)
                        {
                            return;
                        }

                        var msgInfo = _channelService.GetMessage(channelId, messageStatus!.MessageId);

                        if(msgInfo.Status == Models.MessageStatus.Seen)
                        {
                            return;
                        }

                        if(msgInfo.Status == Models.MessageStatus.Delivered && messageStatus.Status == Models.MessageStatus.Delivered)
                        {
                            return;
                        }

                        Models.Chat.MessageStatus msgStatus = new Models.Chat.MessageStatus()
                        {
                            MessageId = messageStatus.MessageId,
                            Status = Models.MessageStatus.Saved,
                        };

                        if (messageStatus.Status == Models.MessageStatus.Delivered)
                        {                       
                            _channelService.UpdateMessageStatus(channelId, messageStatus.MessageId, Models.MessageStatus.Delivered);

                            // inform the user, currently I am not able to find the user that send the message
                            // I need to change the structure the find the user
                            // So I am sending the message to all the user

                            msgStatus.Status = Models.MessageStatus.Delivered;
                        }
                        
                        if (messageStatus.Status == Models.MessageStatus.Seen)
                        {
                            _channelService.UpdateMessageStatus(channelId, messageStatus.MessageId, Models.MessageStatus.Seen);
                            msgStatus.Status = Models.MessageStatus.Seen;
                        }

                        var msgStatusResponse = new WebSocketResponse<Models.Chat.MessageStatus>("MessageStatus", msgStatus);

                        await SendMessageAsync(_channelService.GetConnections(channelId), JsonSerializer.Serialize(msgStatusResponse));
                    }
                    break;
            }
        } 
    }
}
