using CimpleChat.Models;

namespace CimpleChat.Services
{ 
    // reserve (1 bit) + ticks (47 bit) + server id (8 bit) + sequence no (8 bit)
    public class GetNextId: IGetNextId
    {
        private readonly DateTime StartPeriod;
        private readonly int ServerId;
        private int UserSequence;
        private int ChannelSequence;
        private int MessageSequence;

        public GetNextId()
        {
            StartPeriod = new DateTime(2024, 1, 1);
            ServerId = 1;

            UserSequence = 1;
            ChannelSequence = 1;
            MessageSequence = 1;
        }

        public long GetUserId()
        {
            if (UserSequence >= 255) UserSequence = 0;
            
            return NextId(++UserSequence);
        }

        public long GetChannelId()
        {
            if (ChannelSequence >= 255) ChannelSequence = 0;

            return NextId(++ChannelSequence);
        }

        public long GetMessageId()
        {
            if(MessageSequence >= 255) MessageSequence = 0;

            return NextId(++MessageSequence);
        }

        private long NextId(int sequence)
        {
            long ticks = DateTime.Now.Ticks - StartPeriod.Ticks;

            // keep first bit 0 as reserve bit

            long last47BitOfTicks = (ticks >> (64 - 47));  // Right shift to extract last 17 bits 

            long newId = last47BitOfTicks << 8;                 // Left shift 8 bit to append the server id
            newId |= ServerId;                                  // Append server id

            newId = newId << 8;                                 // Left shift 8 bit to append sequence no
            newId |= sequence;                                  // Append sequence no

            return newId;
        }
    }
}
