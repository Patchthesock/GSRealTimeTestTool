using System;

namespace Models
{
    public class Latency
    {
        public readonly double Lag;
        public readonly double RoundTrip;
        public readonly double Throughput;
        
        public Latency(long pingTime, long pongTime)
        {
            if (pingTime == 0 || pongTime == 0)
            {
                Lag = 0;
                RoundTrip = 0;
                Throughput = 0;
            }
            else
            {
                Lag = TimeSpan.FromTicks(pongTime - pingTime).TotalSeconds;
                RoundTrip = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - pingTime).TotalSeconds;
                
                // Convert 1400 bytes (Packet Limit / Window Size) to bits 1400 * 8 = 14400 bits
                // Maximum network throughput equals the window size divided by the round trip time
                // Divide by 1000 to convert to kbits, round to two decimal places.
                Throughput = Math.Round(14400 / RoundTrip / 1000, 2);
            }
        }
        
        public override string ToString()
        {
            return string.Format("Latency: {0}\nThroughput: {1}\nRound Trip Time: {2}",
                Lag, Throughput, RoundTrip);
        }
    }
}