using System;
using System.Collections.Generic;
using Gui;
using Models;
using Models.LogEntry;
using Services;

namespace Controllers
{
    public class GuiController
    {
        // TODO: Better Initialization
        public GuiController(
            AuthGui authGui,
            SessionGui sessionGui,
            ConnectionGui connectionGui,
            PrefabBuilder prefabBuilder)
        {
            _authService = new AuthGuiService(authGui);
            _connectionService = new ConnectionGuiService(connectionGui);
            _sessionService = new SessionGuiService(sessionGui, new MatchService(), prefabBuilder);
        }

        public void Initialize()
        {
            _connectionService.Initialize();
            
            _authService.Initialize(
                OnEndUserSession,
                (name, userId) => { OnAuthentication(name, userId, true); },   // Registration
                (name, userId) => { OnAuthentication(name, userId, false); },  // Authentication
                (name, userId) => { OnAuthentication(name, userId, false); }); // Device Authentication
            
            _sessionService.Initialize(
                OnSendPing,
                OnStopRtSession,
                OnSendBlankPacket,
                OnStartPingTest,
                OnStartRtSession,
                OnSendUnstructuredPacket);
            
            // Initial Screen Order
            _authService.SetActive(true);
            _sessionService.SetActive(false);
        }

        /**
         * <summary>On Log Entry Received</summary>
         * <param name="l">LogEntry</param>
         */
        public void OnLogEntryReceived(ILogEntry l)
        {
            _sessionService.OnLogEntryReceived(l);
        }
        
        /**
         * <summary>Set Real Time Gui Active</summary>
         * <param name="state">Real Time Session isReady state</param>
         */
        public void SetRealTimeActive(bool state)
        {
            _sessionService.SetActive(true);
        }
        
        #region Subscriptions
        
        /**
         * <summary>Subscribe To On Stop Real Time Session</summary>
         * <param name="onStopSession">Delegate Action</param>
         */
        public void SubscribeToOnStopSession(Action onStopSession)
        {
            if (_stopSessionListeners.Contains(onStopSession)) return;
            _stopSessionListeners.Add(onStopSession);
        }
        
        /**
         * <summary>Subscribe To On Start Real Time Session</summary>
         * <param name="onStartSession">Delegate Action</param>
         */
        public void SubscribeToOnStartSession(Action<RtSession> onStartSession)
        {
            if (_startSessionListeners.Contains(onStartSession)) return;
            _startSessionListeners.Add(onStartSession);
        }

        /**
         * <summary>Subscribe To On Send Blank Packet</summary>
         * <param name="onSendBlankPacket">Delegate Action with int param</param>
         */
        public void SubscribeToOnSendBlankPacket(Action<int> onSendBlankPacket)
        {
            if (_sendBlankPacketListeners.Contains(onSendBlankPacket)) return;
            _sendBlankPacketListeners.Add(onSendBlankPacket);
        }

        /**
         * <summary>Subscribe to on send unstructured packet</summary>
         * <param name="onSendUnstructuredPacket">Delegate Action with int param</param>
         */
        public void SubscribeToOnSendUnstructuredPacket(Action<int> onSendUnstructuredPacket)
        {
            if (_sendUnstructuredPacketListeners.Contains(onSendUnstructuredPacket)) return;
            _sendUnstructuredPacketListeners.Add(onSendUnstructuredPacket);
        }
        
        /**
         * <summary>Subscribe To On Send Timestamp Packet</summary>
         * <param name="onSendTimestampPacket">Delegate Action</param>
         */
        public void SubscribeToOnSendPingPacket(Action onSendPingPacket)
        {
            if (_sendPingPacketListeners.Contains(onSendPingPacket)) return;
            _sendPingPacketListeners.Add(onSendPingPacket);
        }

        /**
         * <summary>Subscribe To On Start Throughput Packet Test</summary>
         * <param name="onThroughputPacketTest>Delegate Action</param>
         */
        public void SubscribeToOnStartPingTest(Action<int, int> onStartPingTest)
        {
            if (_onStartPingTestListeners.Contains(onStartPingTest)) return;
            _onStartPingTestListeners.Add(onStartPingTest);
        }
        #endregion
        
        private void OnAuthentication(string name, string userId, bool isReg)
        {
            _authService.SetActive(false);
            _sessionService.SetActive(true);
            if (isReg) _connectionService.OnRegistration(name, userId);
            else _connectionService.OnAuthentication(name, userId);
        }
        
        private void OnEndUserSession()
        {
            OnStopRtSession();
            _authService.SetActive(true);
            _sessionService.SetActive(false);
            _connectionService.OnEndSession();
        }
        
        private void OnStopRtSession()
        {
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

        private void OnSendUnstructuredPacket(int opCode)
        {
            foreach (var l in _sendUnstructuredPacketListeners) l(opCode);
        }
        
        private void OnSendPing()
        {
            foreach (var l in _sendPingPacketListeners) l();
        }

        private void OnStartPingTest(int packetsPerSecond, int seconds)
        {
            foreach (var l in _onStartPingTestListeners) l(packetsPerSecond, seconds);
        }

        private readonly AuthGuiService _authService;
        private readonly SessionGuiService _sessionService;
        private readonly ConnectionGuiService _connectionService;
        private readonly List<Action> _stopSessionListeners = new List<Action>();
        private readonly List<Action> _sendPingPacketListeners = new List<Action>();
        private readonly List<Action<int>> _sendBlankPacketListeners = new List<Action<int>>();
        private readonly List<Action<int>> _sendUnstructuredPacketListeners = new List<Action<int>>();
        private readonly List<Action<RtSession>> _startSessionListeners = new List<Action<RtSession>>();
        private readonly List<Action<int, int>> _onStartPingTestListeners = new List<Action<int, int>>();
    }
}