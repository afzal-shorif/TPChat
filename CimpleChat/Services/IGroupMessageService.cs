using CimpleChat.Models;
using System.Net.WebSockets;

namespace CimpleChat.Services
{
    public interface IGroupMessageService
    {
        public void AddNewChannel(string name, ChannelType type);
        public IList<ChannelInfo> GetChannelList();
        public IList<Message> GetMessages(int channelId);
        public IList<User> GetUsers(int channelId);
        public void AddUserToChannel(int channelId, int userId);
        public int GetTotalChannel();
        public int GetTotalUser(int channelId);
        public void AddNewConnection(int channelId, WebSocket ws);
        public IList<WebSocket> GetConnections(int channelId);
        Task<MessageResponse> AddNewMessage(int channelId, int userId, string msgString);
        Task<MessageResponse> AddNewAnnounceMessage(int channelId, int userId);
    }
}
