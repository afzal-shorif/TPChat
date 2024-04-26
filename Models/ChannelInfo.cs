namespace CimpleChat.Models
{
    public class ChannelInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int NumberOfUser { get; set; }
        public int NumberOfMessage {  get; set; }
        public DateTime CreatedAt { get; set; }
        public ChannelType Type { get; set; }
    }
}
