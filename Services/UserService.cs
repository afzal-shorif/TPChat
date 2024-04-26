using CimpleChat.Models;

namespace CimpleChat.Services
{
    public class UserService: IUserService
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
                CreatedAt = DateTime.Now,
            };

            Users.Add(user);

            return user;
        }

        public IList<User>GetUsers() { return Users; }
        public User GetUser(long userId)
        {
            return Users.Where(u => u.Id == userId).First();
        }
    }
}
