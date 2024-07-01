using CimpleChat.Models;
using CimpleChat.Models.Chat;
using CimpleChat.Models.Channel;
using CimpleChat.Services.UserService;
using System.Net.WebSockets;

namespace CimpleChat.Services.ChannelService
{
    public class GroupMessageService : IGroupMessageService
    {
        #region Fields

        private Dictionary<long, ChannelModel> Channels;
        private Dictionary<long, List<WebSocket>> ActiveConnections;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IGetNextId _getNextId;

        #endregion

        #region Ctor

        public GroupMessageService(IUserService userService, IConfiguration configuration, IGetNextId getNextId)
        {
            Channels = new Dictionary<long, ChannelModel>();
            ActiveConnections = new Dictionary<long, List<WebSocket>>();

            long channelId = getNextId.GetChannelId();
            var ch = new ChannelModel()
            {
                ChannelId = channelId,
                Name = "Test",
                Messages = new List<Message>(),
                Users = new List<long>(),
                CreatedAt = DateTime.Now.AddDays(-1),
                Type = ChannelType.@public
            };

            Channels[channelId] = ch;

            _userService = userService;
            _configuration = configuration;
            _getNextId = getNextId;
        }

        #endregion

        #region Channel

        public void AddNewChannel(string name, ChannelType type)
        {
            if (Channels.Count < 100)
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
            }
        }

        public IList<ChannelInfo> GetChannelList()
        {
            var result = Channels.Select(ch =>
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
            var messages = Channels[channelId].Messages.OrderByDescending(m => m.Id).Take(10);
            var users = _userService.GetUsers();

            var result = messages.Join(users, msg => msg.From, user => user.Id, (msg, user) => new MessageResponse()
            {
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

        public void UpdateMessageStatus(long channelId, long messageId, Models.MessageStatus Status)
        {
            var message = Channels[channelId].Messages.Where(m => m.Id == messageId);
            if (message.Any())
            {
                message.FirstOrDefault<Message>().Status = Status;
            }
        }

        #region User

        public void AddUserToChannel(long channelId, long userId)
        {
            try
            {
                if (GetTotalUser(channelId) < 100)
                {
                    if (!Channels[channelId].Users.Contains(userId))
                    {
                        Channels[channelId].Users.Add(userId);
                    }
                }
                else
                {
                    throw new Exception("User number has exit the limit.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
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

        public void AddNewConnection(long channelId, WebSocket ws)
        {
            if (!ActiveConnections.ContainsKey(channelId))
            {
                ActiveConnections.Add(channelId, new List<WebSocket>());
            }

            ActiveConnections[channelId].Add(ws);
        }

        public void RemoveConnection(long channelId, WebSocket ws)
        {
            if (ActiveConnections.ContainsKey(channelId))
            {
                ActiveConnections[channelId].Remove(ws);
            }
        }

        public IList<WebSocket> GetConnections(long channelId)
        {
            return ActiveConnections[channelId];
        }

        public async Task<MessageResponse> AddNewMessage(MessageRequest messageRequest)
        {
            var user = _userService.GetUser(messageRequest.UserId);

            var msgObj = new Message()
            {
                Id = _getNextId.GetMessageId(),
                From = messageRequest.UserId,
                Content = messageRequest.Content,
                Status = Models.MessageStatus.Saved,
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
                Status = Models.MessageStatus.Saved,
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

        #endregion
    }
}
