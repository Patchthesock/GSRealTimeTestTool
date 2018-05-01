using System;

namespace Models.LogEntry
{
    public class MatchFoundLog : ILogEntry
    {
        private readonly DateTime _createdAt;
        private readonly RtSession _rtSession;
        
        public MatchFoundLog(RtSession rtSession)
        {
            _rtSession = rtSession;
            _createdAt = DateTime.UtcNow;
        }
        
        public string GetTitle()
        {
            return "Match Found :: Inbound :: " + _rtSession.MatchId;
        }

        public string GetFullInfo()
        {
            return _rtSession.ToString();
        }

        public DateTime GetCreatedAt()
        {
            return _createdAt;
        }

        public RtSession GetRtSession()
        {
            return _rtSession;
        }

        public Directions GetDirection()
        {
            return Directions.Inbound;
        }

        public LogEntryTypes GetLogEntryType()
        {
            return LogEntryTypes.MatchFound;
        }
    }
}