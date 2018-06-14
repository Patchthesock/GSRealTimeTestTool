using System.Text;

namespace Models
{
    public class PingTestResults
    {
        public PingTestResults(
            int pingsSent,
            int pongsReceived,
            double averageKBits,
            double averageLatency,
            double averageRoundTripTime)
        {
            _pingsSent = pingsSent;
            _averageKBits = averageKBits;
            _pongsReceived = pongsReceived;
            _averageLatency = averageLatency;
            _averageRoundTripTime = averageRoundTripTime;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine($"Pings Sent: {_pingsSent}")
                .AppendLine($"Pongs Received: {_pongsReceived}")
                .AppendLine($"Average kbits: {_averageKBits}")
                .AppendLine($"Average Latency: {_averageLatency}")
                .AppendLine($"Average Round Trip Time: {_averageRoundTripTime}")
                .ToString();
        }
        
        private readonly int _pingsSent;
        private readonly int _pongsReceived;
        private readonly double _averageKBits;
        private readonly double _averageLatency;
        private readonly double _averageRoundTripTime;
    }
}