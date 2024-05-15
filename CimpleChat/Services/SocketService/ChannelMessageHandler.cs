using CimpleChat.Models;
using CimpleChat.Models.SocketResponse;
using CimpleChat.Services.ChannelService;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

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
            var messageResponse = new WebSocketResponse<MessageResponse<IList<SingleMessageResponse>>>("Message", messages);

            await SendMessageAsync(ws, JsonSerializer.Serialize(messageResponse));

            // save and send join (announce) message
            var connectionList = _channelService.GetConnections(channelId);

            var announced = await _channelService.AddNewAnnounceMessage(channelId, user.Id, "join");
            var announceResponse = new WebSocketResponse<MessageResponse<AnnouncedMessageResponse>>("Message", announced);

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
            var announceResponse = new WebSocketResponse<MessageResponse<AnnouncedMessageResponse>>("Message", announce);

            await SendMessageAsync(connectionList, JsonSerializer.Serialize(announceResponse));

            // send updated user list
            var activeUserList = _channelService.GetActiveUsers(channelId);
            var userListResponse = new WebSocketResponse<IList<ActiveUserResponse>>("ActiveChannelUsers", activeUserList);

            await SendMessageAsync(connectionList, JsonSerializer.Serialize(userListResponse));
        }

        public override async Task ReceiveAsync(WebSocket ws, int channelId, User user)
        {
            byte[] byteArray = new byte[1024];

            while (ws.State == WebSocketState.Open)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(byteArray);
                var response = await ws.ReceiveAsync(buffer, CancellationToken.None);

                if (response.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(byteArray, 0, response.Count);
                    var result = await _channelService.AddNewMessage(channelId, user.Id, message);

                    var socketResponse = new WebSocketResponse<MessageResponse<SingleMessageResponse>>("Message", result);

                    await SendMessageAsync(_channelService.GetConnections(channelId), JsonSerializer.Serialize(socketResponse));
                }
                else if (response.MessageType == WebSocketMessageType.Close)
                {
                    await OnDisconnectAsync(ws, channelId, user);
                }
            }

            if(ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted) { 
                await OnDisconnectAsync(ws, channelId, user);
            }
        }
    }
}
