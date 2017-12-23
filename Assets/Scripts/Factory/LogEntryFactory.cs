using Models;

namespace Factory
{
    public static class LogEntryFactory
    {
        public static LogEntry CreateLeaveSessionLogEntry()
        {
            return new LogEntry("Disconnected From Session", null,
                LogEntry.Directions.Outbound, new PacketDetails(0, 0, 0, 0));
        }
        
        public static LogEntry CreatePingSentEntryLog(int opCode)
        {
            return new LogEntry("Sending Ping Packet", null,
                LogEntry.Directions.Outbound, new PacketDetails(opCode, 0, 0, 0));
        }

        public static LogEntry CreatePongSentEntryLog(int opCode)
        {
            return new LogEntry("Sending Pong Packet", null, LogEntry.Directions.Outbound,
                new PacketDetails(opCode, 0, 0, 0));
        }

        public static LogEntry CreateBlankPacketLogEntry(int opCode)
        {
            return new LogEntry("Sending Blank Packet", null,
                LogEntry.Directions.Outbound, new PacketDetails(opCode, 0, 0, 0));
        }

        public static LogEntry CreatePeerConnectedLogEntry(int peerId)
        {
            return new LogEntry("Peer " + peerId + " Connected", null,
                LogEntry.Directions.Inbound, new PacketDetails(0, 0, 0, 0));
        }

        public static LogEntry CreatePeerDisconnectedLogEntry(int peerId)
        {
            return new LogEntry("Peer " + peerId + " Disconnected", null,
                LogEntry.Directions.Inbound, new PacketDetails(0, 0, 0, 0));
        }

        public static LogEntry CreateRealTimeSessionStateLogEntry(bool state)
        {
            return new LogEntry("Real Time Ready: " + state, null,
                LogEntry.Directions.Inbound, new PacketDetails(0, 0, 0, 0));
        }

        public static LogEntry CreateBlankPacketReceviedLogEntry(PacketDetails p)
        {
            return new LogEntry("Blank Packet Received",
                new Latency(0, 0), LogEntry.Directions.Inbound, p);
        }

        public static LogEntry CreatePingReceivedLogEntry(PacketDetails packet)
        {
            return new LogEntry("Ping Packet Received",
                new Latency(0, 0), LogEntry.Directions.Inbound, packet);
        }

        public static LogEntry CreatePongReceivedLogEntry(long pingTime, long pongTime, PacketDetails packet)
        {
            return new LogEntry("Pong Packet Received",
                new Latency(pingTime, pongTime), LogEntry.Directions.Inbound, packet);
        }
    }
}