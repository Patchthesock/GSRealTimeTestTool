using System;
using System.Collections.Generic;
using Factory;
using Models;
using Models.LogEntry;
using Services;
using UnityEngine;

namespace Gui.Service
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
            _sessionGui.MatchMakingGui.SetActive(true);
            _matchService.Initialize();
            
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
            if (state) return;
            _sessionGui.LogGui.ClearLog();
            _sessionGui.LogEntryInspGui.SetInspectionText("");
            _sessionGui.MatchMakingGui.ClearRealTimeSessionKeys();
        }
        
        /**
         * <summary>On Log Entry Received</summary>
         * <param name="l">Log Entry</param>
         */
        public void OnLogEntryReceived(ILogEntry l)
        {
            CreateLogEntry(l);
            if (l.GetLogEntryType() == LogEntryTypes.OnSessionReady)
            {
                _sessionGui.RealTimeControlGui.SetActive(true);
            }
        }

        private void OnStopRtSession(Action onStop)
        {
            onStop();
            _sessionGui.RealTimeControlGui.SetActive(false);
        }
        
        private void SetupMatchService(Action<RtSession> onStartRtSession)
        {
            InitializeMatchServiceSubscriptions();
            _sessionGui.MatchMakingGui.Initialize(rtSessionKey =>
            {
                OnJoinSession(rtSessionKey, onStartRtSession);
            }, OnFindMatch);
        }

        private void InitializeMatchServiceSubscriptions()
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
        }

        private void OnFindMatch(int skill, string shortCode)
        {
            _matchService.FindMatch(skill, shortCode, (err) => // on match making Error
            {
                OnLogEntryReceived(LogEntryFactory.CreateMatchMakingErrorLogEntry(err));
            });
            OnLogEntryReceived(LogEntryFactory.CreateMatchMakingRequestLogEntry(skill, shortCode));
        }

        private void OnJoinSession(string rtSessionKey, Action<RtSession> onStartRtSession)
        {
            if (!_sessionListDict.ContainsKey(rtSessionKey)) return;
            onStartRtSession(_sessionListDict[rtSessionKey]);
        }
        
        private void CreateLogEntry(ILogEntry l)
        {
            var e = _prefabBuilder.Instantiate(_sessionGui.LogGui.LogEntryPrefab);
            e.transform.SetParent(_sessionGui.LogGui.LogContainer);
            e.transform.localScale = new Vector3(1, 1, 1);
            e.GetComponent<LogEntryBtn>().Initialize(l.GetTitle(), () =>
            {
                _sessionGui.LogEntryInspGui.SetInspectionText(l.GetFullInfo());
            });
        }
        
        private readonly SessionGui _sessionGui;
        private readonly MatchService _matchService;
        private readonly PrefabBuilder _prefabBuilder;
        private readonly Dictionary<string, RtSession> _sessionListDict = new Dictionary<string, RtSession>();
    }
}