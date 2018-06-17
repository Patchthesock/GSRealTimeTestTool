using System;
using System.Collections.Generic;
using Factory;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using Models;
using Models.LogEntry;

namespace Services
{
    public class SparkRtService
    {
        public SparkRtService(Settings settings, GameSparksRTUnity gameSparksRtUnity)
        {
            _rtConnected = false;
            _settings = settings;
            _gameSparksRtUnity = gameSparksRtUnity;
        }
        
        /**
         * <summary>Leave the Real Time Session</summary>
         */
        public void LeaveSession()
        {
            if (!_rtConnected) return;
            _rtConnected = false;
            _gameSparksRtUnity.Disconnect();
            OnLogEntry(LogEntryFactory.CreateLeaveSessionLogEntry());
        }
        
        /**
         * <summary>Send Blank Packet</summary>
         * <param name="opCode">OpCode to send the packet on</param>
         */
        public void SendBlankPacket(int opCode)
        {
            if (!_rtConnected) return;
            var r = GetNextRequestId();
            SendPacket(opCode, _settings.Protocol, PacketDataFactory.GetEmpty(r));
            OnLogEntry(LogEntryFactory.CreateBlankSentLogEntry(r, opCode));
        }

        /**
         * <summary>Send Unstructured Packet</summary>
         * <param name="opCode">OpCode to send the packet on</param>
         */
        public void SendUnstructuredDataPacket(int opCode)
        {
            if (!_rtConnected) return;
            var r = GetNextRequestId();
            SendPacket(opCode, _settings.Protocol, PacketDataFactory.GetUnstructuredData(r));
            OnLogEntry(LogEntryFactory.CreateBlankSentLogEntry(r, opCode));
        }
        
        /**
         * <summary>Send Ping</summary>
         * <remark>This ping is intented to measure the latency and
         * round trip of the active clients in the real time session.
         * This is not a reflection of the client / server connection.</remark>
         */
        public void SendPing()
        {
            if (!_rtConnected) return;
            var r = GetNextRequestId();
            SendPacket((int) OpCode.Ping, _settings.Protocol, PacketDataFactory.GetTimestampPing(r));
            OnLogEntry(LogEntryFactory.CreatePingSentEntryLog(r, (int) OpCode.Ping));
        }
        
        /**
         * <summary>Subscribe to on Real Time Session ready</summary>
         * <param name="onRtReady">Delegate Action with a bool state param</param>
         */
        public void SubscribeToOnRtReady(Action<bool> onRtReady)
        {
            if (_onRtReady.Contains(onRtReady)) return;
            _onRtReady.Add(onRtReady);
        }
        
        /**
         * <summary>Subscribe To On LogEntry Received</summary>
         * <param name="onTimestampPingReceived">Delegate Action with LogEntry param</param>
         */
        public void SubscribeToOnLogEntryReceived(Action<ILogEntry> onLogEntryReceived)
        {
            if (_onLogEntryReceivedListeners.Contains(onLogEntryReceived)) return;
            _onLogEntryReceivedListeners.Add(onLogEntryReceived);
        }
        
        /**
         * <summary>Given RtSession details will establish the Real Time Session connection</summary>
         * <param name="s">RtSession details of the Real Time session to connect to</param>
         */
        public void ConnectSession(RtSession s)
        {
            _gameSparksRtUnity.Configure(
                
                // Note a MatchFoundMessage can also be used here.
                // Gets the message needed to configure a real time session.
                GetMatchFoundResponse(s),

                // OnPlayerConnected / Disconnected Callbacks
                peerId => { OnLogEntry(LogEntryFactory.CreatePeerConnectedLogEntry(peerId)); },
                peerId => { OnLogEntry(LogEntryFactory.CreatePeerDisconnectedLogEntry(peerId)); },
                
                state => // OnRtReady Callback
                {
                    OnLogEntry(LogEntryFactory.CreateSessionStateLogEntry(state));
                    foreach (var l in _onRtReady) l(state);
                },
                
                packet => // OnPacketReceived Callback
                {
                    switch (packet.OpCode)
                    {
                        case (int) OpCode.Ping:
                            OnPingReceived(packet);
                            return;
                        case (int) OpCode.Pong:
                            OnPongReceived(packet);
                            return;
                        default:
                            OnPacketReceived(packet);
                            return;
                    }
                });
            
            _rtConnected = true;
            _gameSparksRtUnity.Connect(); // Connect
            OnLogEntry(LogEntryFactory.CreateSessionJoinLogEntry());
        }
        
        private enum OpCode
        {
            Ping = 998,
            Pong = 999
        }
        
        private void SendPacket(int opCode, GameSparksRT.DeliveryIntent intent, RTData data)
        {
            _gameSparksRtUnity.SendData(opCode, intent, data);
        }

        private void SendPacket(int opCode, GameSparksRT.DeliveryIntent intent, ArraySegment<byte> data)
        {
            _gameSparksRtUnity.SendBytes(opCode, intent, data);
        }
        
        private int GetNextRequestId()
        {
            _requestIdCounter++;
            if (_requestIdCounter >= int.MaxValue - 1) _requestIdCounter = 0;
            return _requestIdCounter;
        }
        
        private void SendPongpacket(int pingRequestId, long pingTime)
        {
            SendPacket((int) OpCode.Pong, _settings.Protocol,
                PacketDataFactory.GetTimestampPong(pingRequestId, pingTime));
            
            OnLogEntry(LogEntryFactory.CreatePongSentEntryLog(pingRequestId, (int) OpCode.Pong));
        }
        
        private void OnPacketReceived(RTPacket p)
        {
             if (p.Data != null) OnStructuredPacketReceived(p);
             else if (p.Stream != null && p.Stream.CanRead) OnUnstructuredPacketReceived(p);
        }

        private void OnStructuredPacketReceived(RTPacket p)
        {
            OnLogEntry(LogEntryFactory.CreateBlankReceviedLogEntry(new PacketDetails(p)));
        }

        private void OnUnstructuredPacketReceived(RTPacket p)
        {
            OnLogEntry(LogEntryFactory.CreateBlankReceviedLogEntry(new PacketDetails(p)));
        }
        
        private void OnPingReceived(RTPacket p)
        {
            if (p.Data == null) return;
            var r = p.Data.GetInt(1);
            var t = p.Data.GetLong(2);
            if (r == null || t == null) return;
            OnLogEntry(LogEntryFactory.CreatePingReceivedLogEntry(new PacketDetails(p)));
            SendPongpacket((int) r, (long) t);
        }
        
        private void OnPongReceived(RTPacket p)
        {
            if (p.Data == null) return;
            var l = p.Data.GetLong(2);
            var j = p.Data.GetLong(3);
            if (l == null || j == null) return;
            OnLogEntry(LogEntryFactory.CreatePongReceivedLogEntry((long) l, (long) j, new PacketDetails(p)));
        }
        
        private void OnLogEntry(ILogEntry e)
        {
            foreach (var l in _onLogEntryReceivedListeners) l(e);
        }

        private static FindMatchResponse GetMatchFoundResponse(RtSession s)
        {
            return new FindMatchResponse(new GSRequestData()
                .AddString("host", s.HostUrl)
                .AddNumber("port", (double) s.PortId)
                .AddString("accessToken", s.AccessToken));
        }

        private bool _rtConnected;
        private int _requestIdCounter;
        private readonly Settings _settings;
        private readonly GameSparksRTUnity _gameSparksRtUnity;
        private readonly List<Action<bool>> _onRtReady = new List<Action<bool>>();
        private readonly List<Action<ILogEntry>> _onLogEntryReceivedListeners = new List<Action<ILogEntry>>();
        
        [Serializable]
        public class Settings
        {
            public GameSparksRT.DeliveryIntent Protocol;
        }
    }
}