﻿using System;
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
            SendPacket(opCode, _settings.Protocol, PacketDataFactory.GetEmpty(GetNextRequestId()));
            OnLogEntry(LogEntryFactory.CreateBlankPacketLogEntry(opCode));
        }
        
        /**
         * <summary>Send Timestamp Ping Packet</summary>
         **/
        public void SendPing()
        {
            SendPacket((int) OpCode.Ping, _settings.Protocol, PacketDataFactory.GetTimestampPing(GetNextRequestId()));
            OnLogEntry(LogEntryFactory.CreatePingSentEntryLog((int) OpCode.Ping));
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
                .AddString("host", s.HostUrl)               // we need a 'FindMatchResponse' that 
                .AddNumber("port", (double)s.PortId)        // we can then us to configure a Real 
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
                        case (int) OpCode.Ping:
                            OnPingReceived(packet);
                            break;
                        case (int) OpCode.Pong:
                            OnPongReceived(packet);
                            break;
                        default:
                            OnBlankPacketReceived(packet);
                            break;
                    }
                });
            
            _gameSparksRtUnity.Connect(); // Connect
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
            
            OnLogEntry(LogEntryFactory.CreatePongSentEntryLog((int) OpCode.Pong));
        }
        
        private void OnBlankPacketReceived(RTPacket p)
        {
            if (p.Data == null) return;
            var r = p.Data.GetInt(1);
            if (r == null) return;
            OnLogEntry(LogEntryFactory.CreateBlankPacketReceviedLogEntry(new PacketDetails((int) r, p)));
        }
        
        private void OnPingReceived(RTPacket p)
        {
            if (p.Data == null) return;
            var r = p.Data.GetInt(1);
            var t = p.Data.GetLong(2);
            if (r == null || t == null) return;
            OnLogEntry(LogEntryFactory.CreatePingReceivedLogEntry(new PacketDetails((int) r, p)));
            SendPongpacket((int) r, (long) t);
        }
        
        private void OnPongReceived(RTPacket p)
        {
            if (p.Data == null) return;
            var r = p.Data.GetInt(1);
            var l = p.Data.GetLong(2);
            var j = p.Data.GetLong(3);
            if (r == null | l == null || j == null) return;
            OnLogEntry(LogEntryFactory.CreatePongReceivedLogEntry((long) l, (long) j, new PacketDetails((int) r, p)));
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