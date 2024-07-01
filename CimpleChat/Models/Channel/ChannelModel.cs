namespace CimpleChat.Models.Channel
{
    public enum ChannelType
    {
        @private,
        @public
    }

    public class ChannelModel
    {
        public long ChannelId { get; set; }
        public string Name { get; set; }
        public List<Message> Messages { get; set; }
        public List<long> Users { get; set; }
        public DateTime CreatedAt { get; set; }
        public ChannelType Type { get; set; }
    }
}
