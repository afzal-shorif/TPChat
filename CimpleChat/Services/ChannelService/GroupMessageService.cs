using CimpleChat.Models;
using CimpleChat.Models.MessageModel;
using CimpleChat.Models.Channel;
using CimpleChat.Services.UserService;

namespace CimpleChat.Services.ChannelService
{
    public class GroupMessageService : IGroupMessageService
    {
        #region Fields

        private Dictionary<long, ChannelModel> Channels;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IGetNextId _getNextId;

        #endregion

        #region Ctor

        public GroupMessageService(IUserService userService, IConfiguration configuration, IGetNextId getNextId)
        {
            Channels = new Dictionary<long, ChannelModel>();

            long channelId = 1;
            var ch = new ChannelModel()
            {
                ChannelId = channelId,
                Name = "Cimple Chat Main Channel",
                Messages = new List<Message>(),
                Users = new List<long>(),
                CreatedAt = DateTime.Now.AddDays(-1),
                Type = ChannelType.@public
            };

            Channels[channelId] = ch;

            var nCh = new ChannelModel()
            {
                ChannelId = getNextId.GetChannelId(),
                Name = "New Test Channel",
                Messages = new List<Message>(),
                Users = new List<long>(),
                CreatedAt = DateTime.Now.AddDays(-1),
                Type = ChannelType.@public
            };

            Channels[nCh.ChannelId] = nCh;

            _userService = userService;
            _configuration = configuration;
            _getNextId = getNextId;
        }

        #endregion

        #region Channel

        public long AddNewChannel(string name, ChannelType type)
        {          
            ChannelModel ch = new ChannelModel()
            {
                ChannelId = _getNextId.GetChannelId(),
                Name = name,
                Messages = new List<Message>(),
                Users = new List<long>(),
                CreatedAt = DateTime.Now,
                Type = type
            };

            Channels.Add(ch.ChannelId, ch);

            return ch.ChannelId;
        }

        public IList<ChannelInfo> GetChannelList(long userId)
        {
            var result = Channels.Where(ch => ch.Value.Users.Contains(userId)).Select(ch =>
                new ChannelInfo()
                {
                    Id = ch.Value.ChannelId,
                    Name = ch.Value.Name,
                    NumberOfMessage = ch.Value.Messages.Count,
                    NumberOfUser = ch.Value.Users.Count,
                    CreatedAt = ch.Value.CreatedAt,
                    Type = ch.Value.Type
                }).OrderBy(ch => ch.Name);

            return result.ToList();
        }

        public int GetTotalChannel()
        {
            return Channels.Count;
        }

        #endregion

        public IList<MessageResponse> GetMessages(long channelId)
        {
            var messages = Channels[channelId].Messages.OrderBy(m => m.Id).Take(10);
            var users = _userService.GetUsers();

            var result = messages.Join(users, msg => msg.From, user => user.Id, (msg, user) => new MessageResponse()
            {
                ChannelId = channelId,
                MessageId = msg.Id,
                UserId = user.Id,
                UserName = user.Name,
                Content = msg.Content,
                Status = msg.Status,
                CreatedAt = msg.CreatedAt,
            }).ToList();

            return result;
        }

        public Message? GetMessage(long channelId, long messageId)
        {
            var message = Channels[channelId].Messages.Where(m => m.Id == messageId);
            if (message.Any())
            {
                return message.FirstOrDefault<Message>();
            }

            return null;
        }

        public void UpdateMessageStatus(long channelId, long messageId, MessageStatus Status)
        {
            var message = Channels[channelId].Messages.Where(m => m.Id == messageId);
            if (message.Any())
            {
                message.FirstOrDefault<Message>().Status = Status;
            }
        }

        #region User

        public bool AddUserToChannel(long channelId, long userId)
        {
            try
            {
                if (!Channels[channelId].Users.Contains(userId))
                {
                    Channels[channelId].Users.Add(userId);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool AddMemberToPublicChannel(long channelId, long userId)
        {
            if (Channels[channelId].Type != ChannelType.@public)
            {
                return false;
            }

            if (!Channels[channelId].Users.Contains(userId))
            {
                Channels[channelId].Users.Add(userId);
                return true;
            }

            return false;
        }

        public bool AddMemberToPrivateChannel(long channelId, long userId, long requestedUserId)
        {
            try
            {
                if(Channels[channelId].Type == ChannelType.@public)
                {
                    Channels[channelId].Users.Add(userId);
                    return true;
                }

                if (!Channels[channelId].Users.Contains(requestedUserId) || Channels[channelId].Users.Contains(userId))
                {
                    return false;
                }

                Channels[channelId].Users.Add(userId);

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IList<long> GetChannelUsers(long channelId)
        {
            return Channels[channelId].Users;
        }

        public IList<ActiveUserResponse> GetActiveUsers(long channelId)
        {
            try
            {
                var result = Channels[channelId].Users;

                var userList = _userService.GetUsers().Join(result, user => user.Id, userId => userId, (user, userId) => new ActiveUserResponse()
                {
                    Id = user.Id,
                    Name = user.Name,
                });

                return userList.ToList() ?? new List<ActiveUserResponse>();
            }
            catch (Exception e)
            {
                return new List<ActiveUserResponse>();
            }
        }

        public int GetTotalUser(long channelId)
        {
            try
            {
                int result = Channels[channelId].Users.Count;

                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public void RemoveUser(long channelId, long userId)
        {
            if (Channels.ContainsKey(channelId))
            {
                Channels[channelId].Users.Remove(userId);
            }
        }

        #endregion

        #region Web Socket

        public async Task<MessageResponse> AddNewMessage(MessageRequest messageRequest)
        {
            var user = _userService.GetUser(messageRequest.UserId);

            var msgObj = new Message()
            {
                Id = _getNextId.GetMessageId(),
                From = messageRequest.UserId,
                Content = messageRequest.Content,
                Status = MessageStatus.Saved,
                CreatedAt = DateTime.Now,
            };

            Channels[messageRequest.ChannelId].Messages.Add(msgObj);

            var msgResponse = new MessageResponse()
            {
                MessageId = msgObj.Id,
                UserId = messageRequest.UserId,
                UserName = user.Name,
                Content = msgObj.Content,
                Status = msgObj.Status,
                CreatedAt = msgObj.CreatedAt,
            };
            
            return msgResponse;
        }

        public async Task<MessageResponse> AddNewAnnounceMessage(long channelId, long userId, string type = "leave")
        {
            var user = _userService.GetUser(userId);

            var msgObj = new Message()
            {
                Id = _getNextId.GetMessageId(),
                From = 0,
                Content = type == "leave" ? $"{user.Name} has leave." : $"{user.Name} has join.",
                Status = MessageStatus.Saved,
                CreatedAt = DateTime.Now,
            };

            Channels[channelId].Messages.Add(msgObj);

            var msgResponse = new MessageResponse()
            {
                MessageId = msgObj.Id,
                UserId = 0,
                UserName = "System",
                Content = msgObj.Content,
                Status = msgObj.Status,
                CreatedAt = DateTime.Now,                
            };

            return msgResponse;
        }

        public IEnumerable<object> SearchChannel(string searchText)
        {
            return Channels.Where(c => c.Value.Name.Contains(searchText) && c.Value.Type == ChannelType.@public).Select(c => new { 
                Id = c.Value.ChannelId ,
                Name = c.Value.Name,
                Type = "channel"
            }).ToList();
        }

        public bool IsChannelNameExist(string name)
        {
            var channels = Channels.Where(c => c.Value.Name == name && c.Value.Type == ChannelType.@public).ToList();

            if(channels == null || channels.Count == 0)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
