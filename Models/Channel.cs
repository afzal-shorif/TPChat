using CimpleChat.Models;

namespace CimpleChat.Models
{
    public enum ChannelType{
        @private,
        @public
    }
    
    public class Channel
    {
        public int ChannelId { get; set; }
        public string Name { get; set; }
        public List<Message> Messages { get; set; }
        public List<int> Users { get; set; }
        public DateTime CreatedAt { get; set; }
        public ChannelType Type { get; set; }
    }
}
