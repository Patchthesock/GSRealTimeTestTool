using System;
using System.Collections.Generic;
using Gui;
using Models;
using Services;
using UnityEngine;

namespace Controllers
{
    public class GuiController
    {
        public GuiController(
            AuthGui authGui,
            MatchGui matchGui,
            SessionGui sessionGui,
            ConnectionGui connectionGui,
            PrefabBuilder prefabBuilder)
        {
            _authService = new AuthService(authGui);
            _matchService = new MatchService(matchGui);
            _connectionService = new ConnectionService(connectionGui);
            _sessionService = new SessionService(sessionGui, prefabBuilder);
        }

        public void Initialize()
        {
            _connectionService.Initialize();
            _matchService.Initialize(OnStartRtSession);
            _authService.Initialize(
                OnUserEndSession,
                (name, userId) => { OnAuthenticationResponseReceived(name, userId, true); },
                (name, userId) => { OnAuthenticationResponseReceived(name, userId); },
                (name, userId) => { OnAuthenticationResponseReceived(name, userId); });
            _sessionService.Initialize(OnStopRtSession, OnSendTimestampPacket, OnSendBlankPacket);
            
            // Initial Screen Order
            _authService.SetActive(true);
            _matchService.SetActive(false);
            _sessionService.SetActive(false);
        }

        /**
         * <summary>On Log Entry Received</summary>
         * <param name="l">LogEntry</param>
         **/
        public void OnLogEntryReceived(LogEntry l)
        {
            _sessionService.OnLogEntryReceived(l);
        }
        
        #region GameSpark General Realtime
        
        /**
         * <summary>Set Real Time Gui Active</summary>
         * <param name="state">Real Time Session isReady state</param>
         **/
        public void SetRealTimeActive(bool state)
        {
            _sessionService.SetActive(true);
        }
        #endregion
        
        #region Subscriptions
        
        /**
         * <summary>Subscribe To On Stop Real Time Session</summary>
         * <param name="onStopSession">Delegate Action</param>
         **/
        public void SubscribeToOnStopSession(Action onStopSession)
        {
            if (_stopSessionListeners.Contains(onStopSession)) return;
            _stopSessionListeners.Add(onStopSession);
        }
        
        /**
         * <summary>Subscribe To On Start Real Time Session</summary>
         * <param name="onStartSession">Delegate Action</param>
         **/
        public void SubscribeToOnStartSession(Action<RtSession> onStartSession)
        {
            if (_startSessionListeners.Contains(onStartSession)) return;
            _startSessionListeners.Add(onStartSession);
        }

        /**
         * <summary>Subscribe To On Send Blank Packet</summary>
         * <param name="onSendBlankPacket">Delegate Action with int param</param>
         **/
        public void SubscribeToOnSendBlankPacket(Action<int> onSendBlankPacket)
        {
            if (_sendBlankPacketListeners.Contains(onSendBlankPacket)) return;
            _sendBlankPacketListeners.Add(onSendBlankPacket);
        }
        
        /**
         * <summary>Subscribe To On Send Timestamp Packet</summary>
         * <param name="onSendTimestampPacket">Delegate Action</param>
         **/
        public void SubscribeToOnSendTimestampPacket(Action onSendTimestampPacket)
        {
            if (_sendTimestampPacketListeners.Contains(onSendTimestampPacket)) return;
            _sendTimestampPacketListeners.Add(onSendTimestampPacket);
        }

        /**
         * <summary>Subscribe To On Start Throughput Packet Test</summary>
         * <param name="onThroughputPacketTest>Delegate Action</param>
         **/
        public void SubscribeToOnStartThroughputPacketTest(Action onThroughputPacketTest)
        {
            if (_throughputPacketTestListeners.Contains(onThroughputPacketTest)) return;
            _throughputPacketTestListeners.Add(onThroughputPacketTest);
        }
        #endregion
        
        private void OnAuthenticationResponseReceived(string name, string userId, bool isReg = false)
        {
            _authService.SetActive(false);
            _matchService.SetActive(true);
            if (isReg) _connectionService.OnRegistration(name, userId);
            else _connectionService.OnAuthentication(name, userId);
        }
        
        private void OnUserEndSession()
        {
            OnStopRtSession();
            _authService.SetActive(true);
            _matchService.SetActive(false);
            _sessionService.SetActive(false);
            _connectionService.OnEndSession();
        }
        
        private void OnStopRtSession()
        {
            _matchService.SetActive(true);
            _sessionService.SetActive(false);
            foreach (var l in _stopSessionListeners) l();
        }
        
        private void OnStartRtSession(RtSession s)
        {
            foreach (var l in _startSessionListeners) l(s);
        }
        
        private void OnSendBlankPacket(int opCode)
        {
            foreach (var l in _sendBlankPacketListeners) l(opCode);
        }
        
        private void OnSendTimestampPacket()
        {
            foreach (var l in _sendTimestampPacketListeners) l();
        }

        private readonly AuthService _authService;
        private readonly MatchService _matchService;
        private readonly SessionService _sessionService;
        private readonly ConnectionService _connectionService;
        private readonly List<Action> _stopSessionListeners = new List<Action>();
        private readonly List<Action> _sendTimestampPacketListeners = new List<Action>();
        private readonly List<Action> _throughputPacketTestListeners = new List<Action>();
        private readonly List<Action<int>> _sendBlankPacketListeners = new List<Action<int>>();
        private readonly List<Action<RtSession>> _startSessionListeners = new List<Action<RtSession>>();
        
        [Serializable]
        public class Settings
        {
            public GameObject LogContainer;
            public GameObject LogEntryPrefab;
        }
    }
}