using Models;
using Models.LogEntry;

namespace Factory
{
    public static class LogEntryFactory
    {
        public static ILogEntry CreateMatchMakingRequestLogEntry(int skill, string shortCode)
        {
            return new SimpleLogEntry(
                $"Match Making Request :: Skill {skill} - ShortCode {shortCode}",
                Directions.Outbound, LogEntryTypes.MatchMakingRequest);
        }

        public static ILogEntry CreateMatchFoundLogEntry(RtSession rtSession)
        {
            return new MatchFoundLog(rtSession);
        }

        public static ILogEntry CreateMatchNotFoundLogEntry()
        {
            return new SimpleLogEntry("Match Not Found", Directions.Inbound, LogEntryTypes.MatchNotFound);
        }

        public static ILogEntry CreateMatchMakingErrorLogEntry(string err)
        {
            return new SimpleLogEntry(err, Directions.Inbound, LogEntryTypes.MatchNotFound);
        }
        
        public static ILogEntry CreatePingSentEntryLog(int requestId, int opCode)
        {
            return new SimpleLogEntry(
                new PacketDetails(opCode, 0, 0, requestId).ToString(),
                Directions.Outbound, LogEntryTypes.PingPacket);
        }

        public static ILogEntry CreatePongSentEntryLog(int requestId, int opCode)
        {
            return new SimpleLogEntry(
                new PacketDetails(opCode, 0, 0, requestId).ToString(),
                Directions.Outbound, LogEntryTypes.PongPacket);
        }

        public static ILogEntry CreateBlankSentLogEntry(int requestId, int opCode)
        {
            return new SimpleLogEntry(
                new PacketDetails(opCode, 0, 0, requestId).ToString(),
                Directions.Outbound, LogEntryTypes.BlankPacket);
        }

        public static ILogEntry CreateSessionJoinLogEntry()
        {
            return new SimpleLogEntry(
                "Joining Real Time Session", Directions.Outbound, LogEntryTypes.OnSessionJoin);
        }
        
        public static ILogEntry CreateSessionStateLogEntry(bool state)
        {
            return new SimpleLogEntry(
                $"Real Time Ready: {state}", Directions.Inbound,
                state ? LogEntryTypes.OnSessionReady : LogEntryTypes.OnSessionNotReady);
        }
        
        public static ILogEntry CreatePeerConnectedLogEntry(int peerId)
        {
            return new SimpleLogEntry(
                $"Peer {peerId} Connected",Directions.Inbound, LogEntryTypes.OnPlayerConnect);
        }

        public static ILogEntry CreatePeerDisconnectedLogEntry(int peerId)
        {
            return new SimpleLogEntry(
                $"Peer {peerId} Disconnected", Directions.Inbound, LogEntryTypes.OnPlayerDisconnect);
        }

        public static ILogEntry CreateLeaveSessionLogEntry()
        {
            return new SimpleLogEntry(
                "Disconnected From Session", Directions.Outbound, LogEntryTypes.OnSessionLeave);
        }
        
        public static ILogEntry CreateBlankReceviedLogEntry(PacketDetails p)
        {
            return new SimpleLogEntry(p.ToString(), Directions.Inbound, LogEntryTypes.BlankPacket);
        }

        public static ILogEntry CreatePingReceivedLogEntry(PacketDetails p)
        {
            return new SimpleLogEntry(p.ToString(), Directions.Inbound, LogEntryTypes.PingPacket);
        }

        public static ILogEntry CreatePongReceivedLogEntry(long pingTime, long pongTime, PacketDetails packet)
        {
            return new PongPacketLog(new Latency(pingTime, pongTime), Directions.Inbound, packet);
        }

        public static ILogEntry CreateQosTestResultsLogEntry(PingTestResults r)
        {
            return new SimpleLogEntry(r.ToString(), Directions.Inbound, LogEntryTypes.QualityOfServiceTestResult);
        }
    }
}