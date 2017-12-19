using System;
using System.Collections.Generic;
using GameSparks.RT;
using Models;
using Services;

namespace Controllers
{
    public class PacketController
    {
        public PacketController(
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
            SendPacket(opCode, _settings.Protocol, PacketService.GetEmptyPacket());
        }
        
        /**
         * <summary>Send Timestamp Ping Packet</summary>
         **/
        public void SendTimestampPingPacket()
        {
            SendPacket((int) OpCode.TimestampPing, _settings.Protocol, PacketService.GetTimestampPingPacket());
        }

        /**
         * <summary>Send Timestamp Pong Packet</summary>
         * <param name="pingTime">Ping time from initial ping packet</param>
         **/
        public void SendTimestampPongpacket(long pingTime)
        {
            SendPacket((int) OpCode.TimestampPong, _settings.Protocol, PacketService.GetTimestampPongPacket(pingTime));
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
        
        #region Subscriptions
        
        /**
         * <summary>Subscribe To On Timestamp Ping Received</summary>
         * <param name="onTimestampPingReceived">Delegate Action with long param</param>
         **/
        public void SubscribeToOnTimestampPingReceived(Action<long> onTimestampPingReceived)
        {
            if (_onTimestampPingReceivedListeners.Contains(onTimestampPingReceived)) return;
            _onTimestampPingReceivedListeners.Add(onTimestampPingReceived);
        }
        
        /**
         * <summary>Subscribe To On Blank Packet Received</summary>
         * <param name="onBlankPacketReceived">Delegate Action with PacketDetails param</param>
         **/
        public void SubscribeToOnBlankPacketReceived(Action<PacketDetails> onBlankPacketReceived)
        {
            if (_onBlankPacketReceivedListeners.Contains(onBlankPacketReceived)) return;
            _onBlankPacketReceivedListeners.Add(onBlankPacketReceived);
        }

        /**
         * <summary>Subscribe To On Timestamp Pong Received</summary>
         * <param name="onTimestampPongReceivied">Delegate Action with Latency and PacketDetails params</param>
         **/
        public void SubscribeToOnTimestampPongReceived(Action<Latency, PacketDetails> onTimestampPongReceived)
        {
            if (_onTimestampPongReceivedListeners.Contains(onTimestampPongReceived)) return;
            _onTimestampPongReceivedListeners.Add(onTimestampPongReceived);
        }
        #endregion
        
        private void OnReceivedBlankPacket(RTPacket packet)
        {
            var p = ModelFactory.CreatePacketDetails(packet);
            foreach (var l in _onBlankPacketReceivedListeners) l(p);
        }
        
        private void OnReceivedTimestampPingPacket(RTPacket packet)
        {
            var p = packet.Data.GetLong(1);
            if (p == null) return;
            foreach (var l in _onTimestampPingReceivedListeners) l((long)p);
        }
        
        private void OnReceivedTimestampPongPacket(RTPacket packet)
        {
            var l = packet.Data.GetLong(1);
            var j = packet.Data.GetLong(2);
            if (l == null || j == null) return;
            var d = ModelFactory.CreatePacketDetails(packet);
            var p = ModelFactory.CreateLatency((long) l, (long) j);
            foreach (var q in _onTimestampPongReceivedListeners) q(p, d);
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

        private readonly Settings _settings;
        private readonly SparkRtService _sparkRtService;
        private readonly List<Action<long>> _onTimestampPingReceivedListeners = new List<Action<long>>();
        private readonly List<Action<PacketDetails>> _onBlankPacketReceivedListeners = new List<Action<PacketDetails>>();
        private readonly List<Action<Latency, PacketDetails>> _onTimestampPongReceivedListeners = new List<Action<Latency, PacketDetails>>();
        
        [Serializable]
        public class Settings
        {
            public GameSparksRT.DeliveryIntent Protocol;
        }
    }
}