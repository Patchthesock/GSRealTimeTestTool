using System;
using System.IO;
using System.Text;
using GameSparks.RT;

namespace Models
{
    public class PacketDetails
    {
        private readonly int _size;
        private readonly int _opCode;
        private readonly int _sender;
        private readonly int _requestId;
        private readonly string _stream;
        
        public PacketDetails(RTPacket p)
        {            
            _opCode = p.OpCode;
            _sender = p.Sender;
            _size = p.PacketSize;
            _stream = GetPacketStream(p);
            _requestId = GetPacketRequestId(p);
        }

        public PacketDetails(int opCode, int sender, int size, int requestId)
        {
            _size = size;
            _opCode = opCode;
            _sender = sender;
            _stream = string.Empty;
            _requestId = requestId;
        }
        
        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendLine($"OpCode: {_opCode}");
            s.AppendLine($"Sender: Peer {_sender}");
            s.AppendLine($"Request ID: {_requestId}");
            s.AppendLine($"Packet Size: {_size} bytes");
            return _stream == string.Empty ? s.ToString() : s.AppendLine($"Stream: {_stream}").ToString();
        }

        private static int GetPacketRequestId(RTPacket p)
        {
            if (p.Data != null) return p.Data.GetInt(1) ?? 0;
            if (p.Stream == null || !p.Stream.CanRead) return 0;
            return p.Stream.ReadByte();
        }
        
        private static string GetPacketStream(RTPacket p)
        {
            if (p.Stream == null || !p.Stream.CanRead) return string.Empty;
            return $"{Convert.ToString(new BinaryReader(p.Stream).ReadByte(), 2).PadLeft(8, '0')}";
        }
    }
}