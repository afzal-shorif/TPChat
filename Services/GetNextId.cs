namespace CimpleChat.Services
{
    public static class GetNextId
    {
        //private int nextChannelId = 0;
        //public int NextChannelId { get { return ++nextChannelId; } }

        //private int nextChannelMessageId = 0;
        //public int NextChannelMessageId { get { return ++nextChannelMessageId; }}

        //private int nextMessageId = 0;
        //public int NextMessageId { get { return ++nextChannelId; } }

        //private int nextUserId = 0;
        //public int NextUserId { get { return ++nextUserId; } }

        //public GetNextId() {
        //    nextChannelId = 0;
        //    nextChannelMessageId = 0;
        //    nextMessageId = 0;
        //    nextUserId = 0;
        //}

        public static int nextChannelId = 0;
        public static int NextChannelId { get { return ++nextChannelId; } }

        private static int nextChannelMessageId = 0;
        public static int NextChannelMessageId { get { return ++nextChannelMessageId; } }

        private static int nextMessageId = 0;
        public static int NextMessageId { get { return ++nextChannelId; } }

        private static int nextUserId = 0;
        public static int NextUserId { get { return ++nextUserId; } }
    }
}
