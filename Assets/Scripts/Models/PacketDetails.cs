using System.IO;
using System.Text;
using GameSparks.RT;

namespace Models
{
    public class PacketDetails
    {
        public readonly int Size;
        public readonly int OpCode;
        public readonly int Sender;
        public readonly int RequestId;
        public readonly string Stream;
        
        public PacketDetails(RTPacket p)
        {            
            OpCode = p.OpCode;
            Sender = p.Sender;
            Size = p.PacketSize;
            RequestId = GetPacketRequestId(p);
            Stream = GetPacketStream(p);
        }

        public PacketDetails(int opCode, int sender, int size, int requestId)
        {
            Size = size;
            Stream = "\"\"";
            OpCode = opCode;
            Sender = sender;
            RequestId = requestId;
        }

        private static int GetPacketRequestId(RTPacket p)
        {
            if (p.Data != null) return p.Data.GetInt(1) ?? 0;
            if (p.Stream == null || !p.Stream.CanRead) return 0;
            return p.Stream.ReadByte();
        }
        
        private static string GetPacketStream(RTPacket p)
        {
            if (p.Stream == null || !p.Stream.CanRead) return "\"\"";
            var r = new StreamReader(p.Stream);
            var m = r.ReadToEnd();
            return "\"" + m + "\"";
        }
    }
}