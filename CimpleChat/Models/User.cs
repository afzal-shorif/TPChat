namespace CimpleChat.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActiveOn { get; set; }
    }
}
