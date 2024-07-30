using CimpleChat.Models;
using CimpleChat.Models.Channel;
using CimpleChat.Models.Chat;
using CimpleChat.Services.ChannelService;
using CimpleChat.Services.ConnectionService;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CimpleChat.Services.SocketService
{
    public class ChannelMessageHandler : WebSocketHandler
    {
        private readonly IGroupMessageService _channelService;
        private readonly IConnectionService _connectionService;

        public ChannelMessageHandler(IGroupMessageService groupMessageService,
                                     IConnectionService connectionService) : base()
        {
            _channelService = groupMessageService;
            _connectionService = connectionService;
        }

        public override async Task OnConnectAsync(WebSocket ws, User user)
        {
            // store the user connection
            _connectionService.AddConnection(user.Id, ws);

            // add the user to the common channel (channel id = 1)
            _channelService.AddUserToChannel(1, user.Id);

            //// save and send join (announce) message
            //var connections = _connectionService.GetConnections(1);

            //var announced = await _channelService.AddNewAnnounceMessage(1, user.Id, "join");
            //var announceResponse = new WebSocketResponse<MessageResponse>("Announce", announced);

            //await SendMessageAsync(
            //    connections.Where(con => con.UserId != user.Id).Select(con => con.connection).ToList(), 
            //    JsonSerializer.Serialize(announceResponse)
            //    );

            // send the common channels (channel id = 1)
            await SendChannelList(ws, user.Id);
        }

        public override async Task OnDisconnectAsync(WebSocket ws, User user)
        {
            // remove connection and user from channel
            _connectionService.RemoveConnection(user.Id);

            //_channelService.RemoveConnection(1, ws);
            //_channelService.RemoveUser(1, user.Id);

            //// send leave message
            //var connectionList = _channelService.GetConnections(1);
            //var announce = await _channelService.AddNewAnnounceMessage(1, user.Id, "leave");
            //var announceResponse = new WebSocketResponse<MessageResponse>("Message", announce);

            //await SendMessageAsync(connectionList, JsonSerializer.Serialize(announceResponse));

            //// send updated user list
            //var activeUserList = _channelService.GetActiveUsers(1);
            //var userListResponse = new WebSocketResponse<IList<ActiveUserResponse>>("ActiveChannelUsers", activeUserList);

            //await SendMessageAsync(connectionList, JsonSerializer.Serialize(userListResponse));

        }

        public override async Task ReceiveAsync(WebSocket ws, User user)
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

                    await ProcessClientMessage(ws, user, message);
                }
                else if (response.MessageType == WebSocketMessageType.Close)
                {
                    isCloseMessageReceived = true;
                    await OnDisconnectAsync(ws, user);
                }
            }

            if (!isCloseMessageReceived)
            {
                ws.Abort();               
                await OnDisconnectAsync(ws, user);
            }
        }


        public async ValueTask ProcessClientMessage(WebSocket ws, User user, string clientResponse)
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

                        await ProcessMessageRequest(ws, user, msgInfo);
                    }
                                    
                    break;
                case "MessageStatus":
                    {
                        Models.Chat.MessageStatus? messageStatus = (JsonSerializer.Deserialize<WebSocketResponse<Models.Chat.MessageStatus>>(clientResponse, option))?.Data;

                        if (messageStatus == null)
                        {
                            return;
                        }

                        await ProcessMessageStatusRequest(messageStatus);
                    }
                    break;
                case "MessageHistory":
                    {
                        MessageHistoryRequest? messageStatus = (JsonSerializer.Deserialize<WebSocketResponse<MessageHistoryRequest>>(clientResponse, option))?.Data;

                        if (messageStatus == null)
                        {
                            return;
                        }

                        await ProcessMessageHistoryRequest(ws, user, messageStatus);
                    }
                    break;
                case "ChannelList":
                    {
                        await SendChannelList(ws, user.Id);
                    }
                    break;
            }
        }
        private async Task ProcessMessageRequest(WebSocket ws, User user, MessageRequest msgInfo)
        {
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

            var connections = (_connectionService.GetConnections(msgInfo.ChannelId)).Where(c => c.UserId != user.Id).Select(c => c.connection).ToList();

            await SendMessageAsync(connections, JsonSerializer.Serialize(socketResponse));
        }
        private async Task ProcessMessageStatusRequest(Models.Chat.MessageStatus messageStatus)
        {
            var msgInfo = _channelService.GetMessage(messageStatus.ChannelId, messageStatus!.MessageId);

            if (msgInfo.Status == Models.MessageStatus.Seen)
            {
                return;
            }

            if (msgInfo.Status == Models.MessageStatus.Delivered && messageStatus.Status == Models.MessageStatus.Delivered)
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
                _channelService.UpdateMessageStatus(messageStatus.ChannelId, messageStatus.MessageId, Models.MessageStatus.Delivered);

                // inform the user, currently I am not able to find the user that send the message
                // I need to change the structure the find the user
                // So I am sending the message to all the user

                msgStatus.Status = Models.MessageStatus.Delivered;
            }

            if (messageStatus.Status == Models.MessageStatus.Seen)
            {
                _channelService.UpdateMessageStatus(messageStatus.ChannelId, messageStatus.MessageId, Models.MessageStatus.Seen);
                msgStatus.Status = Models.MessageStatus.Seen;
            }

            var msgStatusResponse = new WebSocketResponse<Models.Chat.MessageStatus>("MessageStatus", msgStatus);
            var connections = (_connectionService.GetConnections(messageStatus.ChannelId)).Select(c => c.connection).ToList();

            await SendMessageAsync(connections, JsonSerializer.Serialize(msgStatusResponse));
        }
        
        private async Task ProcessMessageHistoryRequest(WebSocket ws, User user, MessageHistoryRequest messageHistReq)
        {
            var messages = _channelService.GetMessages(messageHistReq.ChannelId);

            var socketResponse = new WebSocketResponse<IList<MessageResponse>>("Message", messages);

            await SendMessageAsync(ws, JsonSerializer.Serialize(socketResponse));
        }

        private async Task SendChannelList(WebSocket ws, long userId)
        {
            var channelList = _channelService.GetChannelList(userId);
            var channelListResponse = new WebSocketResponse<IList<ChannelInfo>>("ChannelList", channelList);

            await SendMessageAsync(ws, JsonSerializer.Serialize(channelListResponse));
        }
    }
}
