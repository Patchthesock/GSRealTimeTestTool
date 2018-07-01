using System;
using System.Collections.Generic;
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
            
            var byteStream = GetByteStream(p);
            _stream = GetPacketStream(byteStream);
            _requestId = GetPacketRequestId(p, byteStream);
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
            var s = new StringBuilder()
                .AppendLine($"OpCode: {_opCode}")
                .AppendLine($"Sender: Peer {_sender}")
                .AppendLine($"Request ID: {_requestId}")
                .AppendLine($"Packet Size: {_size} bytes");
            
            return (_stream == string.Empty ? s : s.AppendLine()
                .AppendLine("--- Stream Content ---")
                .AppendLine($"{_stream}")).ToString();
        }

        private static int GetPacketRequestId(RTPacket p, IReadOnlyList<byte> b)
        {
            if (p.Data != null) return p.Data.GetInt(1) ?? 0;
            return b.Count <= 0 ? 0 : b[0];
        }
        
        private static string GetPacketStream(IReadOnlyList<byte> b)
        {
            if (b.Count <= 0) return string.Empty;
            
            var s = new StringBuilder();
            for (var i = 0; i < b.Count; i++)
            {
                s.AppendLine($"Byte {i}: {Convert.ToString(b[i], 2).PadLeft(8, '0')}");
            }
            return s.ToString();
        }

        private static byte[] GetByteStream(RTPacket p)
        {
            if (p.Stream == null || !p.Stream.CanRead) return new byte[0];
            
            var b = new byte[p.StreamLength];
            var br = new BinaryReader(p.Stream);
            for (var i = 0; i < p.StreamLength; i++) b[i] = br.ReadByte();
            return b;
        }
    }
}