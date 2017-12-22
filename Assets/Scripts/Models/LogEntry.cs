using System;

namespace Models
{
    public class LogEntry
    {
        public readonly string Message;
        public readonly DateTime CreatedAt;
        public readonly Directions Direction;
        public readonly Latency LatencyDetail;
        public readonly PacketDetails PacketDetail;
        
        public LogEntry(
            string message,
            Latency latency,
            Directions direction,
            PacketDetails packetDetail)
        {
            Message = message;
            Direction = direction;
            LatencyDetail = latency;
            CreatedAt = DateTime.Now;
            PacketDetail = packetDetail;
        }

        public enum Directions
        {
            Inbound,
            Outbound
        }
    }
}