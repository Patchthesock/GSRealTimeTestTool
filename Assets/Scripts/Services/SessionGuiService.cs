using System;
using System.Collections.Generic;
using Factory;
using Gui;
using Models;
using Models.LogEntry;
using UnityEngine;

namespace Services
{
    public class SessionGuiService
    {
        public SessionGuiService(
            SessionGui sessionGui,
            MatchService matchService,
            PrefabBuilder prefabBuilder)
        {
            _sessionGui = sessionGui;
            _matchService = matchService;
            _prefabBuilder = prefabBuilder;
        }

        public void Initialize(
            Action onSendPing,
            Action onStopRtSession,
            Action<int> onSendBlankPacket,
            Action<int, int> onStartPingTest,
            Action<RtSession> onStartRtSession,
            Action<int> onSendUnstructuredPacket)
        {
            SetupMatchService(onStartRtSession);
            _matchService.Initialize();
            _sessionGui.MatchMakingGui.SetActive(true);
            
            _sessionGui.RealTimeControlGui.Initialize(
                () => { OnStopRtSession(onStopRtSession); },
                onSendPing, onSendBlankPacket, onStartPingTest, onSendUnstructuredPacket);
            _sessionGui.RealTimeControlGui.SetActive(false);
        }
        
        /**
         * <summary>Set Active</summary>
         * <param name="state">State</param>
         */
        public void SetActive(bool state)
        {   
            _sessionGui.SetActive(state);
        }
        
        /**
         * <summary>On Log Entry Received</summary>
         * <param name="l">Log Entry</param>
         */
        public void OnLogEntryReceived(ILogEntry l)
        {
            var e = _prefabBuilder.Instantiate(_sessionGui.LogGui.LogEntryPrefab);
            e.transform.SetParent(_sessionGui.LogGui.LogContainer);
            e.transform.localScale = new Vector3(1, 1, 1);
            e.GetComponent<LogEntryBtn>().Initialize(l.GetTitle(), () =>
            {
                _sessionGui.LogEntryInspGui.SetInspectionText(l.GetFullInfo());
            });
        }

        private void OnStopRtSession(Action onStop)
        {
            onStop();
            _sessionGui.RealTimeControlGui.SetActive(false);
        }
        
        private void SetupMatchService(Action<RtSession> onStartRtSession)
        {
            _matchService.SubscribeToOnMatchNotFound(() =>
            {
                OnLogEntryReceived(LogEntryFactory.CreateMatchNotFoundLogEntry());
            });
            _matchService.SubscribeToOnMatchFound(rtSession =>
            {
                _sessionListDict.Add(rtSession.MatchId, rtSession);
                _sessionGui.MatchMakingGui.AddRealTimeSessionKey(rtSession.MatchId);
                OnLogEntryReceived(LogEntryFactory.CreateMatchFoundLogEntry(rtSession));
            });
            _sessionGui.MatchMakingGui.Initialize(
                rtSessionKey =>
                {
                    if (!_sessionListDict.ContainsKey(rtSessionKey)) return;
                    onStartRtSession(_sessionListDict[rtSessionKey]);
                    _sessionGui.RealTimeControlGui.SetActive(true);
                },
                (skill, shortCode) =>
                {
                    _matchService.FindMatch(skill, shortCode);
                    OnLogEntryReceived(LogEntryFactory.CreateMatchMakingRequestLogEntry(skill, shortCode));
                });
        }
        
        private readonly SessionGui _sessionGui;
        private readonly MatchService _matchService;
        private readonly PrefabBuilder _prefabBuilder;
        private readonly Dictionary<string, RtSession> _sessionListDict = new Dictionary<string, RtSession>();
    }
}