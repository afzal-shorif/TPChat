using CimpleChat.Models;
using CimpleChat.Models.Chat;
using CimpleChat.Models.Channel;
using System.Net.WebSockets;

namespace CimpleChat.Services.ChannelService
{
    public interface IGroupMessageService
    {
        public void AddNewChannel(string name, ChannelType type);
        public IList<ChannelInfo> GetChannelList();
        public IList<MessageResponse> GetMessages(long channelId);
        public Message GetMessage(long channelId, long messageId);
        public void UpdateMessageStatus(long channelId, long messageId, Models.MessageStatus Status);
        public IList<ActiveUserResponse> GetActiveUsers(long channelId);
        public void AddUserToChannel(long channelId, long userId);
        public void RemoveConnection(long channelId, WebSocket ws);
        public int GetTotalChannel();
        public int GetTotalUser(long channelId);
        public void RemoveUser(long channelId, long userId);
        public void AddNewConnection(long channelId, WebSocket ws);
        public IList<WebSocket> GetConnections(long channelId);
        Task<MessageResponse> AddNewMessage(MessageRequest messageRequest);
        Task<MessageResponse> AddNewAnnounceMessage(long channelId, long userId, string type);
    }
}
