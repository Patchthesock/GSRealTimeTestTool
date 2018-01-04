namespace Models
{
    public class PingTestResults
    {
        public readonly int PingsSent;
        public readonly int PongsReceived;
        public readonly double AverageKBits;
        public readonly double AverageLatency;
        public readonly double AverageRoundTripTime;

        public PingTestResults(
            int pingsSent,
            int pongsReceived,
            double averageKBits,
            double averageLatency,
            double averageRoundTripTime)
        {
            PingsSent = pingsSent;
            AverageKBits = averageKBits;
            PongsReceived = pongsReceived;
            AverageLatency = averageLatency;
            AverageRoundTripTime = averageRoundTripTime;
        }
    }
}