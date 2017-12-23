using System;

namespace Models
{
    public class Latency
    {
        public readonly double Lag;
        public readonly double Speed;
        public readonly double RoundTrip;
        public readonly DateTime PingTime;
        public readonly DateTime PongTime;
        
        public Latency(long pingTime, long pongTime)
        {
            PingTime = new DateTime(pingTime);
            PongTime = new DateTime(pongTime);
            if (pingTime == 0 || pongTime == 0)
            {
                Lag = 0;
                Speed = 0;
                RoundTrip = 0;
            }
            else
            {
                Lag = TimeSpan.FromTicks(pongTime - pingTime).TotalSeconds;
                RoundTrip = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - pingTime).TotalSeconds;
                
                // Convert 1400 bytes (Packet Limit / Window Size) to bits 1400 * 8 = 14400 bits
                // Maximum network throughput equals the window size divided by the round trip time
                // Divide by 1000 to convert to kbits, round to two decimal places.
                Speed = Math.Round(14400 / RoundTrip / 1000, 2);
            }
        }
    }
}