using System;

namespace Models.LogEntry
{
    public class PongPacketLog : ILogEntry
    {
        private readonly Latency _latency;
        private readonly DateTime _createdAt;
        private readonly Directions _direction;
        private readonly PacketDetails _packetDetails;
        
        public PongPacketLog(
            Latency latency,
            Directions direction,
            PacketDetails packetDetails)
        {
            _latency = latency;
            _direction = direction;
            _createdAt = DateTime.UtcNow;
            _packetDetails = packetDetails;
        }
        
        public string GetTitle()
        {
            return $"{LogEntryTypes.PongPacket} :: {_direction} :: {_createdAt}";
        }

        public string GetFullInfo()
        {
            return _packetDetails + _latency.ToString();
        }

        public Latency GetLatency()
        {
            return _latency;
        }

        public DateTime GetCreatedAt()
        {
            return _createdAt;
        }

        public Directions GetDirection()
        {
            return _direction;
        }

        public LogEntryTypes GetLogEntryType()
        {
            return LogEntryTypes.PongPacket;
        }
    }
}