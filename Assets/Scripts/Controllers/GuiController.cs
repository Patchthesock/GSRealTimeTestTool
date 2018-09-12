using System;
using System.Collections.Generic;
using GameSparks.Core;
using Gui;
using Gui.Service;
using Models;
using Models.LogEntry;
using UnityEngine;

namespace Controllers
{
    public class GuiController
    {
        public GuiController(
            ConnectionGui connectionGui,
            AuthGuiService authGuiService,
            SessionGuiService sessionGuiService)
        {
            _connectionGui = connectionGui;
            _authGuiService = authGuiService;
            _sessionGuiService = sessionGuiService;
        }

        public void Initialize()
        {
            SetInitialScreenDisplay();
            InitializeSessionService();
            _connectionGui.gameObject.SetActive(true);
            GS.GameSparksAvailable += r => { _connectionGui.SetActive(!r); Debug.Log(string.Format($"GameSparksAvailable: {r}")); };
            
            _authGuiService.Initialize(() =>
            { // OnAuthentication
                _sessionGuiService.SetActive(true);
            }, () => 
            { // OnLogOut
                OnStopRtSession();
                _sessionGuiService.SetActive(false);
            });
        }

        /**
         * <summary>On Log Entry Received</summary>
         * <param name="l">LogEntry</param>
         */
        public void OnLogEntryReceived(ILogEntry l)
        {
            _sessionGuiService.OnLogEntryReceived(l);
        }
        
        /**
         * <summary>Set Real Time Gui Active</summary>
         * <param name="state">Real Time Session isReady state</param>
         */
        public void SetRealTimeActive(bool state)
        {
            _sessionGuiService.SetActive(true);
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

        private void SetInitialScreenDisplay()
        {
            _sessionGuiService.SetActive(false);
            _connectionGui.gameObject.SetActive(false);
        }

        private void InitializeSessionService()
        {
            _sessionGuiService.Initialize(
                OnSendPing,
                OnStopRtSession,
                OnSendBlankPacket,
                OnStartPingTest,
                OnStartRtSession,
                OnSendUnstructuredPacket);
        }
        
        private void OnSendPing()
        {
            foreach (var l in _sendPingPacketListeners) l();
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

        private void OnStartPingTest(int packetsPerSecond, int seconds)
        {
            foreach (var l in _onStartPingTestListeners) l(packetsPerSecond, seconds);
        }

        private readonly ConnectionGui _connectionGui;
        private readonly AuthGuiService _authGuiService;
        private readonly SessionGuiService _sessionGuiService;
        private readonly List<Action> _stopSessionListeners = new List<Action>();
        private readonly List<Action> _sendPingPacketListeners = new List<Action>();
        private readonly List<Action<int>> _sendBlankPacketListeners = new List<Action<int>>();
        private readonly List<Action<int>> _sendUnstructuredPacketListeners = new List<Action<int>>();
        private readonly List<Action<RtSession>> _startSessionListeners = new List<Action<RtSession>>();
        private readonly List<Action<int, int>> _onStartPingTestListeners = new List<Action<int, int>>();
    }
}