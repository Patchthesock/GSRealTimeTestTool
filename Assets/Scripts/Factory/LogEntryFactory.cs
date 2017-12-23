using Models;

namespace Factory
{
    public static class LogEntryFactory
    {
        /**
         * <summary>Create a Log</summary>
         * <param name="message">Log Message</param>
         * <param name="packetDetails">Packet Details</param>
         * <param name="direction">Log Entry Direction</param>
         **/
        public static LogEntry Create(string message, PacketDetails packetDetails, LogEntry.Directions direction)
        {
            return new LogEntry(message, null, direction, packetDetails);
        }
        
        /**
         * <summary>Create a Log</summary>
         * <param name="message">Log Message</param>
         * <param name="packetDetails">Packet Details</param>
         * <param name="latency">Latency</param>
         * <param name="direction">Log Entry Direction</param>
         **/
        public static LogEntry Create(string message, PacketDetails packetDetails, Latency latency, LogEntry.Directions direction)
        {
            return new LogEntry(message, latency, direction, packetDetails);
        }

        public static LogEntry CreateLeaveSessionLogEntry()
        {
            return new LogEntry("Disconnected From Session", null,
                LogEntry.Directions.Outbound, new PacketDetails(0, 0, 0, 0));
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
    }
}