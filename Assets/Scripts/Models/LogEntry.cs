using System;

namespace Models
{
    public class LogEntry
    {
        public readonly DateTime Time;
        public readonly string Message;
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
            Time = DateTime.Now;
            Direction = direction;
            LatencyDetail = latency;
            PacketDetail = packetDetail;
        }

        public enum Directions
        {
            Inbound,
            Outbound
        }
    }
}