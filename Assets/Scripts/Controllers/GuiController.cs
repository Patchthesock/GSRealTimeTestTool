using System;
using System.Collections.Generic;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using Gui;
using Models;
using Services;

namespace Controllers
{
    public class GuiController
    {
        public GuiController(
            AuthGui authGui,
            PacketGui packetGui,
            UserInfoGui userInfoGui,
            SparkService sparkService,
            RtSessionGui rtSessionGui,
            MatchDetailsGui matchDetailsGui)
        {
            _authGui = authGui;
            _packetGui = packetGui;
            _userInfoGui = userInfoGui;
            _sparkService = sparkService;
            _rtSessionGui = rtSessionGui;
            _matchDetailsGui = matchDetailsGui;
        }

        /**
         * <summary>Initialize GUI Controller</summary>
         **/
        public void Initialize()
        {
            _authGui.SetActive(true);
            _userInfoGui.Initialize();
            _rtSessionGui.SetActive(false);
            _matchDetailsGui.SetActive(false);
            _rtSessionGui.Initialize(OnStopRtSession);
            _packetGui.Initialize(OnSendTimestampPacket, OnSendBlankPacket);
            _authGui.Initialize(_sparkService, OnUserEndSession, OnRegistrationResponseReceived, OnAuthenticationResponseReceived);
            _matchDetailsGui.Initialize(OnStartRtSession, (skill, matchName) => { _sparkService.FindMatch(matchName, skill, OnMatchMakingResponseReceived); });
        }

        #region GameSparks Connection
        
        /**
         * <summary>On GameSparks Available</summary>
         * <param name="state">GameSparks isAvailable state</param>
         **/
        public void OnGsAvailable(bool state)
        {
            _userInfoGui.OnGsAvailable(state);
        }
        #endregion

        #region GameSpark Match
        
        /**
         * <summary>On Match Found Received</summary>
         * <param name="message">MatchFoundMessage</param>
         **/
        public void OnMatchFound(MatchFoundMessage message)
        {
            _matchDetailsGui.OnMatchFoundReceived(message);
        }

        /**
         * <summary>On Match Not found Recevied</summary>
         * <param name="message">MatchNotFoundMessage</param>
         **/
        public void OnMatchNotFound(MatchNotFoundMessage message)
        {
            _matchDetailsGui.OnMatchNotFoundReceived(message);
        }
        #endregion

        #region GameSpark General Realtime
        
        /**
         * <summary>On Real Time Session Ready</summary>
         * <param name="state">Real Time Session isReady state</param>
         **/
        public void OnRtReady(bool state)
        {
            if (!state) return;
            _rtSessionGui.SetActive(true);
        }
        
        /**
         * <summary>On Real Time Player Connected</summary>
         * <param name="peerId">Connected player peerId</param>
         **/
        public void OnPlayerConnected(int peerId)
        {
            _rtSessionGui.OnPlayerConnect(peerId);
        }
    
        /**
         * <summary>On Real Time Player Disconnected</summary>
         * <param name="peerId">Disconnected player peerId</param>
         **/
        public void OnPlayerDisconnected(int peerId)
        {
            _rtSessionGui.OnPlayerDisconnect(peerId);
        }
        #endregion

        #region Packets Received
        
        /**
         * <summary>On Latency Received</summary>
         * <param name="l">Latency</param>
         * <param name="d>Packet Details</param>
         **/
        public void OnLatencyReceived(Latency l, PacketDetails d)
        {
            _packetGui.OnLatencyReceived(l, d);
        }

        /**
         * <summary>On Blank Packet Received</summary>
         * <param name="p">Packet Details</param>
         **/
        public void OnBlankPacketReceived(PacketDetails p)
        {
            _packetGui.OnBlankPacketRecieved(p);
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
        public void SubscribeToOnStartSession(Action onStartSession)
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

        private void OnStopRtSession()
        {
            _rtSessionGui.SetActive(false);
            _matchDetailsGui.SetActive(true);
            foreach (var l in _stopSessionListeners) l();
        }
        
        private void OnStartRtSession()
        {
            foreach (var l in _startSessionListeners) l();
        }
        
        private void OnRegistrationResponseReceived(RegistrationResponse response)
        {
            _authGui.SetActive(false);
            _matchDetailsGui.SetActive(true);
            _userInfoGui.OnRegistration(response);
        }
        
        private void OnMatchMakingResponseReceived(MatchmakingResponse response)
        {
            if (response.HasErrors) _matchDetailsGui.OnMatchmakingErrorReceived(response.Errors);
        }
        
        private void OnAuthenticationResponseReceived(AuthenticationResponse response)
        {
            _authGui.SetActive(false);
            _matchDetailsGui.SetActive(true);
            _userInfoGui.OnAuthentication(response);
        }
        
        private void OnUserEndSession()
        {
            OnStopRtSession();
            _authGui.SetActive(true);
            _userInfoGui.OnEndSession();
            _rtSessionGui.SetActive(false);
            _matchDetailsGui.SetActive(false);
        }
        
        private void OnSendBlankPacket(int opCode)
        {
            foreach (var l in _sendBlankPacketListeners) l(opCode);
        }
        
        private void OnSendTimestampPacket()
        {
            foreach (var l in _sendTimestampPacketListeners) l();
        }
        
        private readonly AuthGui _authGui;
        private readonly PacketGui _packetGui;
        private readonly UserInfoGui _userInfoGui;
        private readonly SparkService _sparkService;
        private readonly RtSessionGui _rtSessionGui;
        private readonly MatchDetailsGui _matchDetailsGui;
        private readonly List<Action> _stopSessionListeners = new List<Action>();
        private readonly List<Action> _startSessionListeners = new List<Action>();
        private readonly List<Action> _sendTimestampPacketListeners = new List<Action>();
        private readonly List<Action> _throughputPacketTestListeners = new List<Action>();
        private readonly List<Action<int>> _sendBlankPacketListeners = new List<Action<int>>();
    }
}