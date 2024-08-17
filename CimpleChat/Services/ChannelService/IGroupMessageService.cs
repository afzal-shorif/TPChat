using CimpleChat.Models.Channel;
using CimpleChat.Models;
using CimpleChat.Models.MessageModel;

namespace CimpleChat.Services.ChannelService
{
    public interface IGroupMessageService
    {
        public long AddNewChannel(string name, ChannelType type);
        public IList<ChannelInfo> GetChannelList(long userId);
        public IList<MessageResponse> GetMessages(long channelId);
        public Message GetMessage(long channelId, long messageId);
        public void UpdateMessageStatus(long channelId, long messageId, MessageStatus Status);
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
