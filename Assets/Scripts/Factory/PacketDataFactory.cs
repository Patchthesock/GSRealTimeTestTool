using System;
using System.Collections;
using System.Text;
using GameSparks.RT;

namespace Factory
{
    public static class PacketDataFactory
    {
        /**
         * <summary>Returns Empty Packet Data<summary>
         */
        public static RTData GetEmpty(int requestId)
        {
            var data = new RTData();
            data.SetInt(1, requestId); // Fixes WebGL build, can't send blank data.
            return data;
        }
        
        /**
         * <summary>Returns Timestamp Ping Packet Data</summary>
         * <param name="requestId">Request Id</param>
         */
        public static RTData GetTimestampPing(int requestId)
        {
            var data = new RTData();
            data.SetInt(1, requestId);
            data.SetLong(2, DateTime.UtcNow.Ticks);
            return data;
        }

        /**
         * <summary>Returns Timestamp Pong Packet Data</summary>
         * <param name="requestId">Request Id</param>
         * <param name="pingTime">Ping time</param>
         */
        public static RTData GetTimestampPong(int requestId, long pingTime)
        {
            var data = new RTData();
            data.SetInt(1, requestId);
            data.SetLong(2, pingTime);
            data.SetLong(3, DateTime.UtcNow.Ticks);
            return data;
        }
        
        /**
         * <summary>Return a Byte Array Segement</summary>
         */
        public static ArraySegment<byte> GetUnstructuredData(int requestId)
        {
            var b = new byte[2];
            b[0] = (byte) requestId;
            b[1] = byte.MaxValue;
            return new ArraySegment<byte>(b);
        }

    }
}