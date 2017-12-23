using System;
using GameSparks.RT;

namespace Models
{
    public class PacketDetails
    {
        public readonly int Size;
        public readonly int OpCode;
        public readonly int Sender;
        public readonly int RequestId;
        
        public PacketDetails(int requestId, RTPacket p)
        {            
            OpCode = p.OpCode;
            Sender = p.Sender;
            Size = p.PacketSize;
            RequestId = requestId;
        }

        public PacketDetails(int opCode, int sender, int size, int requestId)
        {
            Size = size;
            OpCode = opCode;
            Sender = sender;
            RequestId = requestId;
        }
    }
}