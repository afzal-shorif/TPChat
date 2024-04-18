using CimpleChat.Models;

namespace CimpleChat.Services
{
    public class UserService: IUserService
    {
        private List<User> Users { get; set; }
        
        public UserService() 
        { 
            Users = new List<User>();
        }

        public User AddNewUser(string userName)
        {
            var user = new User()
            {
                Id = GetNextId.NextUserId,
                Name = userName,
                CreatedAt = DateTime.Now,
            };

            Users.Add(user);

            return user;
        }

        public IList<User>GetUsers() { return Users; }
    }
}
