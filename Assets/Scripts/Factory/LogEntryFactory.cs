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
    }
}