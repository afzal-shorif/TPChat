using CimpleChat.Models;
using CimpleChat.Models.SocketResponse;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json.Serialization;

namespace CimpleChat.Services
{
    public class GroupMessageService : IGroupMessageService
    {
        #region Fields

        private Dictionary<int, Channel>Channels;
        private Dictionary<int, List<WebSocket>>ActiveConnections;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IGetNextId _getNextId;

        #endregion

        #region Ctor

        public GroupMessageService(IUserService userService, IConfiguration configuration, IGetNextId getNextId)
        {
            Channels = new Dictionary<int, Channel>();
            ActiveConnections = new Dictionary<int, List<WebSocket>>();

            int channelId = getNextId.GetChannelId();
            var ch = new Channel()
            {
                ChannelId = channelId,
                Name = "Test",
                Messages = new List<Message>(),
                Users = new List<int>(),
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
                Channel ch = new Channel()
                {
                    ChannelId = _getNextId.GetChannelId(),
                    Name = name,
                    Messages = new List<Message>(),
                    Users = new List<int>(),
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

        public MessageResponse<IList<SingleMessageResponse>> GetMessages(int channelId)
        {
            var messages = Channels[channelId].Messages.OrderByDescending(m => m.Id).Take(10);
            var users = _userService.GetUsers();

            var result = messages.Join(users, msg => msg.From, user => user.Id, (msg, user) => new SingleMessageResponse()
            {
                Message = msg,
                User = user
            }).ToList();

            var response = new MessageResponse<IList<SingleMessageResponse>>()
            {
                MessageType = "Multiple",
                MessageInfo = result
            };

            return response;
        }

        #region User

        public void AddUserToChannel(int channelId, int userId)
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

        public IList<ActiveUserResponse> GetActiveUsers(int channelId)
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

        public int GetTotalUser(int channelId)
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

        public void RemoveUser(int channelId, int userId)
        {
            if (Channels.ContainsKey(channelId))
            {
                Channels[channelId].Users.Remove(userId);
            }
        }

        #endregion

        #region Web Socket

        public void AddNewConnection(int channelId, WebSocket ws)
        {
            if (!ActiveConnections.ContainsKey(channelId))
            {
                ActiveConnections.Add(channelId, new List<WebSocket>());
            }

            ActiveConnections[channelId].Add(ws);
        }

        public void RemoveConnection(int channelId, WebSocket ws)
        {
            if (ActiveConnections.ContainsKey(channelId))
            {
                ActiveConnections[channelId].Remove(ws);
            }
        }

        public IList<WebSocket> GetConnections(int channelId)
        {
            return ActiveConnections[channelId];
        }

        public async Task<MessageResponse<SingleMessageResponse>> AddNewMessage(int channelId, int userId, string msgString)
        {
            var user = _userService.GetUser(userId);

            var msgObj = new Message()
            {
                Id = _getNextId.GetMessageId(),
                From = userId,
                Content = msgString,
                Status = MessageStatus.Saved,
                CreatedAt = DateTime.Now,
            };

            Channels[channelId].Messages.Add(msgObj);

            var msgResponse = new MessageResponse<SingleMessageResponse>()
            {
                MessageType = "Single",
                MessageInfo = new SingleMessageResponse()
                {
                    Message = msgObj,
                    User = user
                }  
            };

            return msgResponse;
        }

        public async Task<MessageResponse<AnnouncedMessageResponse>> AddNewAnnounceMessage(int channelId, int userId, string type = "Leave")
        {
            var user = _userService.GetUser(userId);

            var msgObj = new Message()
            {
                Id = _getNextId.GetMessageId(),
                From = 0,
                Content = $"{user.Name} has joined.",
                Status = MessageStatus.Saved,
                CreatedAt = DateTime.Now,
            };

            Channels[channelId].Messages.Add(msgObj);

            var msgResponse = new MessageResponse<AnnouncedMessageResponse>()
            {
                MessageType = "Announce",
                MessageInfo = new AnnouncedMessageResponse()
                {
                    Content = msgObj.Content
                }
            };

            return msgResponse;
        }

        #endregion
    }
}
