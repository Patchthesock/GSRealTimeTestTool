using System;

namespace Models.LogEntry
{
    public class SimpleLogEntry : ILogEntry
    {
        private readonly string _info;
        private readonly DateTime _createdAt;
        private readonly Directions _direction;
        private readonly LogEntryTypes _logType;
        
        public SimpleLogEntry(
            string info,
            Directions direction,
            LogEntryTypes logType)
        {
            _info = info;
            _logType = logType;
            _direction = direction;
            _createdAt = DateTime.UtcNow;
        }

        public string GetTitle()
        {
            return _logType + " :: " + _direction + " :: " + _createdAt;
        }

        public string GetFullInfo()
        {
            //var s = Message + "\nDirection: " + Direction + "\n\n";
            //if (PacketDetail != null) s += PacketDetail.ToString();
            //if (LatencyDetail != null) s += LatencyDetail.ToString();
            //return s;
            return _info;
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
            return _logType;
        }
    }
}