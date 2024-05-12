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

        public int GetUserId()
        {
            if (UserSequence >= 255) UserSequence = 0;
            
            return NextId(++UserSequence);
        }

        public int GetChannelId()
        {
            if (ChannelSequence >= 255) ChannelSequence = 0;

            return NextId(++ChannelSequence);
        }

        public int GetMessageId()
        {
            if(MessageSequence >= 255) MessageSequence = 0;

            return NextId(++MessageSequence);
        }

        private int NextId(int sequence)
        {
            return sequence;
            long ticks = DateTime.Now.Ticks - StartPeriod.Ticks;
            //long id = (ticks * 10) + ServerId;

            //int numOfDigit = (int) Math.Floor(Math.Log10(sequence)) + 1;
            //id *= (long)Math.Pow(10, numOfDigit);
            //id += sequence;

            //return id;

            // keep first bit 0 as reserve bit

            int last19BitOfTicks = (int) (ticks >> (64 - 47));  // Right shift to extract last 15 bits 

            int newId = last19BitOfTicks << 8;                  // Left shift 8 bit to append the server id
            newId |= ServerId;                                  // Append server id

            newId = newId << 8;                                 // Left shift 8 bit to append sequence no
            newId |= sequence;                                  // Append sequence no

            return newId;
        }
    }
}
