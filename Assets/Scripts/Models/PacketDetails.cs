using GameSparks.RT;

namespace Models
{
    public class PacketDetails
    {
        public int Size { get; private set; }
        public int OpCode { get; private set; }
        public int Sender { get; private set; }
        public int StreamLength { get; private set; }
        
        public PacketDetails(RTPacket p)
        {
            OpCode = p.OpCode;
            Sender = p.Sender;
            Size = p.PacketSize;
            StreamLength = p.StreamLength;
        }
    }
}