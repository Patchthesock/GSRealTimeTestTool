using System;
using GameSparks.RT;

namespace Models
{
    public class PacketDetails
    {
        public readonly int Size;
        public readonly int OpCode;
        public readonly int Sender;
        public readonly int StreamLength;
        
        public PacketDetails(RTPacket p)
        {
            OpCode = p.OpCode;
            Sender = p.Sender;
            Size = p.PacketSize;
            StreamLength = p.StreamLength;
        }
    }
}