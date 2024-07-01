using CimpleChat.Models;
using System.Net.WebSockets;

namespace CimpleChat.Repository.ConnectionRepository
{
    public class ConnectionRepository: IConnectionRepository
    {
        #region Fields

        private IList<Connections> Connections { get; set; }

        #endregion

        #region Ctor

        public ConnectionRepository()
        {
            Connections = new List<Connections>();
        }

        #endregion

        #region public Methods

        public void AddConnection(long userId, WebSocket ws)
        {
            Connections.Add(new Models.Connections()
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
