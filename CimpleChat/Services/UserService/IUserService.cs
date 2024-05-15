using CimpleChat.Models;

namespace CimpleChat.Services.UserService
{
    public interface IUserService
    {
        public User AddNewUser(string userName);
        public IList<User> GetUsers();
        public User GetUser(long userId);
        public bool IsUsernameAvailable(string username);
    }
}
