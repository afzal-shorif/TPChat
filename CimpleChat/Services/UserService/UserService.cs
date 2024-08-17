﻿using CimpleChat.Models;

namespace CimpleChat.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IGetNextId _getNextId;
        private List<User> Users { get; set; }

        public UserService(IGetNextId getNextId)
        {
            Users = new List<User>();
            _getNextId = getNextId;
        }

        public User AddNewUser(string userName)
        {
            var user = new User()
            {
                Id = _getNextId.GetUserId(),
                Name = userName,
                CreatedAt = DateTime.UtcNow,
                LastActiveOn = DateTime.UtcNow,
            };

            Users.Add(user);

            return user;
        }

        public IList<User> GetUsers() { return Users; }
        public User GetUser(long userId)
        {
            return Users.Where(u => u.Id == userId).First();
        }

        public bool IsUsernameAvailable(string username)
        {
            User? user = Users.Where(u => u.Name == username).FirstOrDefault();

            return user == null;
        }

        public void UpdateLastActiveOn(long userId)
        {
            Users.Where(u => u.Id == userId).FirstOrDefault()!.LastActiveOn = DateTime.UtcNow;
        }

        public void RemoveInactiveUsers()
        {
            foreach (var user in Users)
            {
                int diff = (user.LastActiveOn - DateTime.UtcNow).Minutes;

                if(diff > 2)
                {
                    Users.Remove(user);
                }
            }
        }

        public IEnumerable<object> SearchUser(string username)
        {
            return Users.Where(u => u.Name.Contains(username)).Select(u => new {
                ID = u.Id,
                Name = u.Name,
            });
        }
    }
}
