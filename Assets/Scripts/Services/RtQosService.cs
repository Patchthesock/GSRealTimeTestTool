using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Factory;
using Models;
using Models.LogEntry;
using UnityEngine;

namespace Services
{
    public class RtQosService
    {
        public RtQosService(AsyncProcessor asyncProcessor)
        {
            _asyncProcessor = asyncProcessor;
        }
        
        public void StartPingTest(int seconds, int packetsPerSecond, Action sendPing)
        {
            if (_activeTest)
            {
                _activeTest = false;
                if (_currentTest != null) _asyncProcessor.StopCoroutine(_currentTest);
                Reset(); return; // If Active, Kill, Reset & Return
            }
            
            if (seconds <= 0 || packetsPerSecond <= 0) return;
            _currentTest = PingTest(packetsPerSecond, DateTime.UtcNow.AddSeconds(seconds), sendPing);
            _asyncProcessor.StartCoroutine(_currentTest);
        }
        
        public void OnLogEntryReceived(ILogEntry l)
        {
            if (!_activeTest) return;
            switch (l.GetLogEntryType())
            {
                case LogEntryTypes.PingPacket:
                {
                    _pingCount++;
                    break;
                }
                case LogEntryTypes.PongPacket:
                {
                    _pongs.Add((PongPacketLog) l);
                    break;
                }
            }
        }

        public void OnSubscribeToPingTestResults(Action<ILogEntry> onPingTestResult)
        {
            if (_pingTestResultsListeners.Contains(onPingTestResult)) return;
            _pingTestResultsListeners.Add(onPingTestResult);
        }

        private IEnumerator PingTest(int packetsPerSecond, DateTime endTime, Action sendPing)
        {
            if (packetsPerSecond <= 0)
            {
                _activeTest = false;
                yield break;
            }
            _activeTest = true;
            while (DateTime.UtcNow <= endTime)
            {
                sendPing();
                yield return new WaitForSeconds((float) 1/packetsPerSecond);
            }
            
            yield return new WaitForSeconds(1);
            _activeTest = false;
            
            WriteOutResults(new PingTestResults(
                _pingCount,
                _pongs.Count,
                GetAverageKBits(_pongs),
                GetAverageLatency(_pongs),
                GetAverageRoundTripTime(_pongs)));
            
            Reset();
        }

        private void WriteOutResults(PingTestResults r)
        {
            var e = LogEntryFactory.CreateQosTestResultsLogEntry(r);
            foreach (var l in _pingTestResultsListeners) l(e);
        }

        private void Reset()
        {
            _pingCount = 0;
            _pongs.Clear();
        }
        
        private static double GetAverage(IList<double> e)
        {
            return !e.Any() ? 0 : e.Average();
        }

        private static double GetAverageKBits(IEnumerable<PongPacketLog> e)
        {
            return GetAverage(e
                .Where(i => i.GetLatency().Speed > 0)
                .Select(i => i.GetLatency().Speed).ToList());
        }

        private static double GetAverageLatency(IEnumerable<PongPacketLog> e)
        {
            return GetAverage(e
                .Where(i => i.GetLatency().Lag > 0)
                .Select(i => i.GetLatency().Lag).ToList());
        }

        private static double GetAverageRoundTripTime(IEnumerable<PongPacketLog> e)
        {
            return GetAverage(e
                .Where(i => i.GetLatency().RoundTrip > 0)
                .Select(i => i.GetLatency().RoundTrip).ToList());
        }

        private int _pingCount;
        private bool _activeTest;
        private IEnumerator _currentTest;
        
        private readonly AsyncProcessor _asyncProcessor;
        private readonly List<PongPacketLog> _pongs = new List<PongPacketLog>();
        private readonly List<Action<ILogEntry>> _pingTestResultsListeners = new List<Action<ILogEntry>>();
    }
}