using GameSparks.Api.Messages;
using GameSparks.RT;

namespace Models
{
    public static class ModelFactory
    {
        public static Latency CreateLatency(long ping, long pong)
        {
            return new Latency(ping, pong);
        }

        public static RtSession CreateRtSession(MatchFoundMessage m)
        {
            return new RtSession(m);
        }

        public static PacketDetails CreatePacketDetails(RTPacket p)
        {
            return new PacketDetails(p);
        }
    }
}