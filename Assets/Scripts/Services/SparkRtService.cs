using System;
using System.Collections.Generic;
using Factory;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using Models;

namespace Services
{
    public class SparkRtService
    {
        public SparkRtService(Settings settings, GameSparksRTUnity gameSparksRtUnity)
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
            OnLogEntry(LogEntryFactory.CreateLeaveSessionLogEntry());
        }
        
        /**
         * <summary>Send Blank Packet</summary>
         * <param name="opCode">OpCode to send the blank packet with</param>
         **/
        public void SendBlankPacket(int opCode)
        {
            SendPacket(opCode, _settings.Protocol, PacketDataFactory.GetEmpty());
            OnLogEntry(LogEntryFactory.CreateBlankPacketLogEntry(opCode));
        }
        
        /**
         * <summary>Send Timestamp Ping Packet</summary>
         **/
        public void SendTimestampPingPacket()
        {
            SendPacket((int) OpCode.TimestampPing, _settings.Protocol,
                PacketDataFactory.GetTimestampPing(GetNextRequestId()));
            
            OnLogEntry(new LogEntry("Sending Ping Packet", null, LogEntry.Directions.Outbound,
                new PacketDetails((int) OpCode.TimestampPing, 0, 0, 0)));
            
            // TODO: Maybe queue each sent
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
            _gameSparksRtUnity.Configure(
                new FindMatchResponse(new GSRequestData()   // In order to create a new RtSession 
                .AddNumber("port", (double)s.PortId)        // we need a 'FindMatchResponse' that 
                .AddString("host", s.HostUrl)               // we can then us to configure a Real 
                .AddString("accessToken", s.AcccessToken)), // Time Session from

                // OnPlayerConnected / Disconnected Callbacks
                peerId => { OnLogEntry(LogEntryFactory.CreatePeerConnectedLogEntry(peerId)); },
                peerId => { OnLogEntry(LogEntryFactory.CreatePeerDisconnectedLogEntry(peerId)); },
                
                state => // OnRtReady Callback
                {
                    OnLogEntry(LogEntryFactory.CreateRealTimeSessionStateLogEntry(state));
                    foreach (var l in _onRtReady) l(state);
                },
                
                packet => // OnPacketReceived Callback
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
            OnLogEntry(new LogEntry("Sending Pong Packet", null, LogEntry.Directions.Outbound,
                new PacketDetails((int) OpCode.TimestampPong, 0, 0, 0)));
        }
        
        private void OnReceivedBlankPacket(RTPacket packet)
        {
            OnLogEntry(LogEntryFactory.Create("Blank Packet Received",
                new PacketDetails(packet), LogEntry.Directions.Inbound));
        }
        
        private void OnReceivedTimestampPingPacket(RTPacket packet)
        {
            var r = packet.Data.GetInt(1);
            var p = packet.Data.GetLong(2);
            if (r == null) return;
            if (p == null) return;
            
            OnLogEntry(LogEntryFactory.Create("Ping Packet Received",
                new PacketDetails(packet), LogEntry.Directions.Inbound));
            SendTimestampPongpacket((int) r, (long) p);
        }
        
        private void OnReceivedTimestampPongPacket(RTPacket packet)
        {
            var l = packet.Data.GetLong(2);
            var j = packet.Data.GetLong(3);
            if (l == null || j == null) return;
            
            OnLogEntry(LogEntryFactory.Create("Pong Packet Received", new PacketDetails(packet),
                new Latency((long) l, (long) j), LogEntry.Directions.Inbound));
        }
        
        private void OnLogEntry(LogEntry e)
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