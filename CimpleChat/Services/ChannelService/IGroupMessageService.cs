using CimpleChat.Models;
using CimpleChat.Models.SocketResponse;
using System.Net.WebSockets;

namespace CimpleChat.Services.ChannelService
{
    public interface IGroupMessageService
    {
        public void AddNewChannel(string name, ChannelType type);
        public IList<ChannelInfo> GetChannelList();
        public MessageResponse<IList<SingleMessageResponse>> GetMessages(int channelId);
        public IList<ActiveUserResponse> GetActiveUsers(int channelId);
        public void AddUserToChannel(int channelId, int userId);
        public void RemoveConnection(int channelId, WebSocket ws);
        public int GetTotalChannel();
        public int GetTotalUser(int channelId);
        public void RemoveUser(int channelId, int userId);
        public void AddNewConnection(int channelId, WebSocket ws);
        public IList<WebSocket> GetConnections(int channelId);
        Task<MessageResponse<SingleMessageResponse>> AddNewMessage(int channelId, int userId, string msgString);
        Task<MessageResponse<AnnouncedMessageResponse>> AddNewAnnounceMessage(int channelId, int userId, string type);
    }
}
