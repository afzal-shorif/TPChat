using CimpleChat.Models;
using System.Linq;

namespace CimpleChat.Services
{
    public class GroupMessageService: IGroupMessageService
    {
        private List<Channel> Channels;
        private readonly IUserService _userService;
        
        public GroupMessageService(IUserService userService)
        {
            Channels = new List<Channel>();

            var c = new Channel()
            {
                ChannelId = 1,
                Name = "Test",
                Messages = new List<Message>(),
                Users = new List<int>(),
                CreatedAt = DateTime.Now.AddDays(-1),
                Type = ChannelType.@public
            };
            Channels.Add(c);
            _userService = userService;
        }

        public void AddNewChannel(Channel channel)
        {
            if(Channels.Count < 100)
            {
                Channels.Add(channel);
            }
            else
            {
                throw new Exception("The channel has exit the limit.");
            }
        }


        public IDictionary<int, string> GetChannelList()
        {
            var result = Channels.Select(c => new { ChannelId = c.ChannelId, ChannelName = c.Name }).OrderBy(c => c.ChannelName);

            return result.ToDictionary(c => c.ChannelId, r => r.ChannelName);
        }

        public IList<Message> GetMessages(int channelId)
        {
            var result = Channels.Where(c => c.ChannelId == channelId).Single().Messages.OrderBy(m => m.Id);

            return result.ToList() ?? new List<Message>();
        }

        public void  AddUserToChannel(int channelId, int userId)
        {
            try
            {
                if(GetTotalUser(channelId) < 100)
                {
                    Channels.Where(c => c.ChannelId == channelId).Single().Users.Add(userId);
                }
                else
                {
                    throw new Exception("User number has exit the limit.");
                }
                
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public IList<User> GetUsers(int channelId)
        {
            try
            {
                //var result = Channels.Where(c => c.ChannelId == channelId).Single().Users.OrderBy(u => u.Name);
                //var result = Channels.Join(_userService.GetUsers(), channel => channel.Users, user => user, (channel, user) => user).Where(c => c.ChannelId == channelId).Single();
                var result = Channels.Where(c => c.ChannelId == channelId).Single().Users.ToList();

                var userList = _userService.GetUsers().Join(result, user => user.Id, userId => userId, (user, userId) =>  user);

                return userList.ToList() ?? new List<User>();
            }
            catch (Exception e)
            {
                return new List<User>();
            }
        }

        public int GetTotalChannel()
        {
            return Channels.Count;
        }

        public int GetTotalUser(int channelId)
        {
            try
            {
                var result = Channels.Where(c => c.ChannelId == channelId).Single().Users.Count;

                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
