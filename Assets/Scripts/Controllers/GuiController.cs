using System;
using System.Collections.Generic;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
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

        public void Initialize()
        {
            _authGui.SetActive(true);
            _userInfoGui.Initialize();
            _rtSessionGui.SetActive(false);
            _matchDetailsGui.SetActive(false);
            _rtSessionGui.Initialize(OnStopRtSession);
            _packetGui.Initialize(OnSendTimestampPacket, OnSendBlankPacket);
            _authGui.Initialize(_sparkService, OnUserEndSession, OnRegistration, OnAuthentication);
            _matchDetailsGui.Initialize(OnStartRtSession, (skill, matchName) => { _sparkService.FindMatch(matchName, skill, OnMatchMaking); });
        }

        #region GameSpark Connection
        public void OnGsAvailable(bool state)
        {
            _userInfoGui.OnGsAvailable(state);
        }
        #endregion

        #region GameSpark Match
        public void OnMatchFound(MatchFoundMessage message)
        {
            _matchDetailsGui.OnMatchFound(message);
        }

        public void OnMatchNotFound(MatchNotFoundMessage message)
        {
            _matchDetailsGui.OnMatchNotFound(message);
        }
        
        private void OnMatchMaking(MatchmakingResponse response)
        {
            if (response.HasErrors) _matchDetailsGui.OnMatchmakingError(response.Errors);
        }
        #endregion

        #region GameSpark General Realtime
        public void OnRtReady(bool isReady)
        {
            if (!isReady) return;
            _rtSessionGui.SetActive(true);
        }
        
        public void OnPlayerConnected(int peerId)
        {
            _rtSessionGui.OnPlayerConnect(peerId);
        }
    
        public void OnPlayerDisconnected(int peerId)
        {
            _rtSessionGui.OnPlayerDisconnect(peerId);
        }
        #endregion

        #region Packets Received
        
        public void OnLatencyReceived(Latency l, PacketDetails d)
        {
            _packetGui.OnLatencyReceived(l, d);
        }

        public void OnBlankPacketReceived(PacketDetails p)
        {
            _packetGui.OnBlackPacketARecieved(p);
        }
        #endregion
        
        #region Subscriptions
        public void SubscribeToOnStopSession(Action onStopSession)
        {
            if (_stopSessionListeners.Contains(onStopSession)) return;
            _stopSessionListeners.Add(onStopSession);
        }
        
        public void SubscribeToOnStartSession(Action onStartSession)
        {
            if (_startSessionListeners.Contains(onStartSession)) return;
            _startSessionListeners.Add(onStartSession);
        }

        public void SubscribeToOnSendBlankPacketA(Action<int> onSendBlankPacket)
        {
            if (_sendBlankPacketListeners.Contains(onSendBlankPacket)) return;
            _sendBlankPacketListeners.Add(onSendBlankPacket);
        }
        
        public void SubscribeToOnSendTimestampPacket(Action onSendTimestampPacket)
        {
            if (_sendTimestampPacketListeners.Contains(onSendTimestampPacket)) return;
            _sendTimestampPacketListeners.Add(onSendTimestampPacket);
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
        
        private void OnRegistration(RegistrationResponse response)
        {
            _authGui.SetActive(false);
            _matchDetailsGui.SetActive(true);
            _userInfoGui.OnRegistration(response);
        }
        
        private void OnAuthentication(AuthenticationResponse response)
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
        private readonly List<Action<int>> _sendBlankPacketListeners = new List<Action<int>>();
    }
}