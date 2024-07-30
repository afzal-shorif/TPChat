using CimpleChat.Models;
using CimpleChat.Models.Chat;
using System.Net.WebSockets;

namespace CimpleChat.Repository.ConnectionRepository
{
    public class ConnectionRepository: IConnectionRepository
    {
        #region Fields

        private IList<Connection> Connections { get; set; }

        #endregion

        #region Ctor

        public ConnectionRepository()
        {
            Connections = new List<Connection>();
        }

        #endregion

        #region public Methods

        public Connection? GetConnection(long userId)
        {
            return Connections.Where(c => c.UserId == userId).FirstOrDefault();
        }

        public IList<Connection> GetConnections(IList<long>users)
        {
            return Connections.Join(users, con => con.UserId, user => user, (con, user) => con).ToList();
        }

        public void AddConnection(long userId, WebSocket ws)
        {
            Connections.Add(new Models.Connection()
            {
                UserId = userId,
                connection = ws,
            });
        }

        public void RemoveConnection(long userId)
        {
            var con = Connections.Where(c => c.UserId == userId).FirstOrDefault();

            if (con != null)
            {
                Connections.Remove(con);
            }
        }

        public void RemoveConnection(WebSocket ws)
        {
            var con = Connections.Where(c => c.connection == ws).FirstOrDefault();

            if (con != null)
            {
                Connections.Remove(con);
            }
        }       

        #endregion
    }
}
