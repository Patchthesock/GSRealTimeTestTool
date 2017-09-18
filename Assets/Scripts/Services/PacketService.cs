using System;
using GameSparks.RT;

namespace Services
{
    public static class PacketService
    {
        public static RTData GetEmptyPacket()
        {
            var data = new RTData();
            data.SetInt(1, 1); // Fixes WebGL, can't send blank data.
            return data;
        }
        
        public static RTData GetTimestampPingPacket()
        {
            var data = new RTData();
            data.SetLong(1, DateTime.UtcNow.Ticks);
            return data;
        }

        public static RTData GetTimestampPongPacket(long pingTime)
        {
            var data = new RTData();
            data.SetLong(1, pingTime);
            data.SetLong(2, DateTime.UtcNow.Ticks);
            return data;
        }
    }
}