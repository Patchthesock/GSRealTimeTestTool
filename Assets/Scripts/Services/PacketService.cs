using System;
using GameSparks.RT;

namespace Services
{
    public static class PacketService
    {
        /**
         * <summary>Returns Empty Packet Data<summary>
         **/
        public static RTData GetEmptyPacket()
        {
            var data = new RTData();
            data.SetInt(1, 1); // Fixes WebGL build, can't send blank data.
            return data;
        }
        
        /**
         * <summary>Returns Timestamp Ping Packet Data</summary>
         **/
        public static RTData GetTimestampPingPacket()
        {
            var data = new RTData();
            data.SetLong(1, DateTime.UtcNow.Ticks);
            return data;
        }

        /**
         * <summary>Returns Timestamp Pong Packet Data</summary>
         * <param name="pingTime">Ping time</param>
         **/
        public static RTData GetTimestampPongPacket(long pingTime)
        {
            var data = new RTData();
            data.SetLong(1, pingTime);
            data.SetLong(2, DateTime.UtcNow.Ticks);
            return data;
        }
    }
}