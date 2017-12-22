using System;
using System.Collections.Generic;
using Factory;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using Models;
using UnityEngine;

namespace Services
{
    public class SparkRtService
    {
        public SparkRtService(
            Settings settings,
            GameSparksRTUnity gameSparksRtUnity)
        {
            _settings = settings;
            _gameSparksRtUnity = gameSparksRtUnity;
        }
        
        /**
         * <summary>Leave the Real Time Session</summary>
         **/
        public void LeaveSession()
        {
            _gameSparksRtUnity.Disconnect();
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
         * <summary>Subscribe to on Real Time Session ready</summary>
         * <param name="onRtReady">Delegate Action with a bool state param</param>
         **/
        public void SubscribeToOnRtReady(Action<bool> onRtReady)
        {
            if (_onRtReady.Contains(onRtReady)) return;
            _onRtReady.Add(onRtReady);
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
         * <summary>Given RtSession details will establish the Real Time Session connection</summary>
         * <param name="s">RtSession details of the Real Time session to connect to</param>
         **/
        public void ConnectSession(RtSession s)
        {
            /**
             * In order to create a new RtSession we need a 'FindMatchResponse'
             * In our case, we wanted to capture these details and have them passed in
             * this offers us greater flexibility.
            **/
            
            _gameSparksRtUnity.Configure(
                new FindMatchResponse(new GSRequestData()  // Construct a FindMatchResponse 
                .AddNumber("port", (double)s.PortId)       // that we can then us to configure
                .AddString("host", s.HostUrl)              // a Real Time Session from
                .AddString("accessToken", s.AcccessToken)),

                (peerId) => // OnPlayerConnected Callback
                {
                    Debug.Log("Player " + peerId + " Connected");
                },
                (peerId) => // OnPlayerDisconnected Callback
                {
                    Debug.Log("Player " + peerId + " Disconnected");
                },
                (state) => // OnRtReady Callback
                {
                    foreach (var l in _onRtReady) l(state);
                },
                (packet) => // OnPacketReceived Callback
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
                });
            _gameSparksRtUnity.Connect(); // Connect
        }
        
        private enum OpCode
        {
            TimestampPing = 998,
            TimestampPong = 999
        }
        
        private void SendPacket(int opCode, GameSparksRT.DeliveryIntent intent, RTData data)
        {
            _gameSparksRtUnity.SendData(opCode, intent, data);
        }
        
        private int GetNextRequestId()
        {
            _requestIdCounter++;
            if (_requestIdCounter >= int.MaxValue - 1) _requestIdCounter = 0;
            return _requestIdCounter;
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
            
            OnLogEntryReceived(LogEntryFactory.Create(
                "Ping Timestamp Received",
                new PacketDetails(packet),
                LogEntry.Directions.Inbound));
            SendTimestampPongpacket((int) r, (long) p);
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

        private int _requestIdCounter;
        private readonly Settings _settings;
        private readonly GameSparksRTUnity _gameSparksRtUnity;
        private readonly List<Action<bool>> _onRtReady = new List<Action<bool>>();
        private readonly List<Action<LogEntry>> _onLogEntryReceivedListeners = new List<Action<LogEntry>>();
        
        [Serializable]
        public class Settings
        {
            public GameSparksRT.DeliveryIntent Protocol;
        }
    }
}