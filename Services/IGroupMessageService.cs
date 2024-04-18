using CimpleChat.Models;

namespace CimpleChat.Services
{
    public interface IGroupMessageService
    {
        public void AddNewChannel(Channel channel);
        public IDictionary<int, string> GetChannelList();
        public IList<Message> GetMessages(int channelId);
        public IList<User> GetUsers(int channelId);
        public int GetTotalChannel();
        public int GetTotalUser(int channelId);
    }
}
