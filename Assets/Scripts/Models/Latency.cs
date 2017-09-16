using System;

namespace Models
{
    public class Latency
    {
        public Latency(long pingTime, long pongTime)
        {
            CurrentTime = DateTime.UtcNow;
            PingTime = new DateTime(pingTime);
            PongTime = new DateTime(pongTime);
            Lag = TimeSpan.FromTicks(pongTime - pingTime).TotalSeconds;
            RoundTrip = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - pingTime).TotalSeconds;
        }

        public double Lag { get; private set; }
        public double RoundTrip { get; private set; }
        public DateTime PingTime { get; private set; }
        public DateTime PongTime { get; private set; }
        public DateTime CurrentTime { get; private set; }
    }
}