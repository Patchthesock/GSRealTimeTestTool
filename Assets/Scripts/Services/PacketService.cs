using System;
using System.Collections.Generic;
using Factory;
using GameSparks.RT;
using Models;

namespace Services
{
    public class PacketService
    {
        public PacketService(
            Settings settings,
            SparkRtService sparkRtService)
        {
            _settings = settings;
            _sparkRtService = sparkRtService;
        }
        
        /**
         * <summary>Send Blank Packet</summary>
         * <param name="opCode">OpCode to send the blank packet with</param>
         **/
        public void SendBlankPacket(int opCode)
        {
            SendPacket(opCode, _settings.Protocol, PacketDataFactory.GetEmpty());
        }
        
        /**
         * <summary>Send Timestamp Ping Packet</summary>
         **/
        public void SendTimestampPingPacket()
        {
            SendPacket(
                (int) OpCode.TimestampPing,
                _settings.Protocol, PacketDataFactory.GetTimestampPing(GetNextRequestId()));
        }
        
        /**
         * <summary>Subscribe To On LogEntry Received</summary>
         * <param name="onTimestampPingReceived">Delegate Action with LogEntry param</param>
         **/
        public void SubscribeToOnLogEntryReceived(Action<LogEntry> onLogEntryReceived)
        {
            if (_onLogEntryReceivedListeners.Contains(onLogEntryReceived)) return;
            _onLogEntryReceivedListeners.Add(onLogEntryReceived);
        }

        /**
         * <summary>On Packet Received</summary>
         * <param name="packet">RTPacket Received</param>
         **/
        public void OnPacketReceived(RTPacket packet)
        {
            switch (packet.OpCode)
            {
                case (int)OpCode.TimestampPing:
                    OnReceivedTimestampPingPacket(packet);
                    break;
                case (int)OpCode.TimestampPong:
                    OnReceivedTimestampPongPacket(packet);
                    break;
                default:
                    OnReceivedBlankPacket(packet);
                    break;
            }
        }
        
        private void SendTimestampPongpacket(int pingRequestId, long pingTime)
        {
            SendPacket(
                (int) OpCode.TimestampPong,
                _settings.Protocol, PacketDataFactory.GetTimestampPong(pingRequestId, pingTime));
        }
        
        private void OnReceivedBlankPacket(RTPacket packet)
        {
            OnLogEntryReceived(LogEntryFactory.Create(
                "Blank Packet Received",
                new PacketDetails(packet),
                LogEntry.Directions.Inbound));
        }
        
        private void OnReceivedTimestampPingPacket(RTPacket packet)
        {
            var r = packet.Data.GetInt(1);
            var p = packet.Data.GetLong(2);
            if (r == null) return;
            if (p == null) return;
            
            SendTimestampPongpacket((int)r, (long)p);
            OnLogEntryReceived(LogEntryFactory.Create(
                "Ping Timestamp Received",
                new PacketDetails(packet),
                LogEntry.Directions.Inbound));
        }
        
        private void OnReceivedTimestampPongPacket(RTPacket packet)
        {
            var l = packet.Data.GetLong(2);
            var j = packet.Data.GetLong(3);
            if (l == null || j == null) return;
            
            OnLogEntryReceived(LogEntryFactory.Create(
                "Pong Timestamp Received",
                new PacketDetails(packet),
                new Latency((long) l, (long) j),
                LogEntry.Directions.Inbound));
        }

        private void OnLogEntryReceived(LogEntry e)
        {
            foreach (var l in _onLogEntryReceivedListeners) l(e);
        }
        
        private void SendPacket(int opCode, GameSparksRT.DeliveryIntent protocol, RTData data)
        {
            _sparkRtService.SendPacket(opCode, protocol, data);
        }

        private enum OpCode
        {
            TimestampPing = 998,
            TimestampPong = 999
        }

        private int GetNextRequestId()
        {
            _requestIdCounter++;
            if (_requestIdCounter >= int.MaxValue - 1) _requestIdCounter = 0;
            return _requestIdCounter;
        }

        private int _requestIdCounter;
        private readonly Settings _settings;
        private readonly SparkRtService _sparkRtService;
        private readonly List<Action<LogEntry>> _onLogEntryReceivedListeners = new List<Action<LogEntry>>();
        
        [Serializable]
        public class Settings
        {
            public GameSparksRT.DeliveryIntent Protocol;
        }
    }
}