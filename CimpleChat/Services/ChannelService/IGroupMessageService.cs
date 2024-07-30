using CimpleChat.Models;
using CimpleChat.Models.Chat;
using CimpleChat.Models.Channel;
using System.Net.WebSockets;

namespace CimpleChat.Services.ChannelService
{
    public interface IGroupMessageService
    {
        public long AddNewChannel(string name, ChannelType type);
        public IList<ChannelInfo> GetChannelList(long userId);
        public IList<MessageResponse> GetMessages(long channelId);
        public Message GetMessage(long channelId, long messageId);
        public void UpdateMessageStatus(long channelId, long messageId, Models.MessageStatus Status);
        public IList<ActiveUserResponse> GetActiveUsers(long channelId);
        bool AddUserToChannel(long channelId, long userId);
        bool AddMemberToPublicChannel(long channelId, long userId);
        bool AddMemberToPrivateChannel(long channelId, long userId, long requestedUserId);
        IList<long> GetChannelUsers(long channelId);
        public int GetTotalChannel();
        public int GetTotalUser(long channelId);
        public void RemoveUser(long channelId, long userId);
        Task<MessageResponse> AddNewMessage(MessageRequest messageRequest);
        Task<MessageResponse> AddNewAnnounceMessage(long channelId, long userId, string type);
        IEnumerable<object> SearchChannel(string searchText);
        bool IsChannelNameExist(string name);
    }
}
