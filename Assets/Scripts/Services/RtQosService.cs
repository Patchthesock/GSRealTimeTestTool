using System;
using System.Collections;
using System.Collections.Generic;
using Models;
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
                Reset();
                return; // If Active, Kill current and return
            }
            
            if (seconds <= 0 || packetsPerSecond <= 0) return;
            _currentTest = PingTest(packetsPerSecond, DateTime.UtcNow.AddSeconds(seconds), sendPing);
            _asyncProcessor.StartCoroutine(_currentTest);
        }
        
        public void OnLogEntryReceived(LogEntry l)
        {
            if (!_activeTest) return;
            switch (l.Direction)
            {
                case LogEntry.Directions.Outbound: // Ping 
                {
                    _pings.Add(l);
                    break;
                }
                case LogEntry.Directions.Inbound:  // Pong
                {
                    _pongs.Add(l);
                    break;
                }
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void OnSubscribeToPingTestResults(Action<PingTestResults> onPingTestResult)
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
                _pings.Count,
                _pongs.Count,
                GetAverageKBits(_pongs),
                GetAverageLatency(_pongs),
                GetAverageRoundTripTime(_pongs)));
            Reset();
        }

        private void WriteOutResults(PingTestResults r)
        {
            foreach (var l in _pingTestResultsListeners) l(r);
        }

        private void Reset()
        {
            _pings.Clear();
            _pongs.Clear();
        }

        private static double GetAverageKBits(IEnumerable<LogEntry> entries)
        {
            var sum = 0.0;
            var count = 0;
            foreach (var e in entries)
                if (e.LatencyDetail != null && Math.Abs(e.LatencyDetail.Speed) > 0)
                {
                    count++;
                    sum += e.LatencyDetail.Speed;
                }
            if (Math.Abs(sum) <= 0 || count == 0) return 0;
            return sum / count;
        }

        private static double GetAverageLatency(IEnumerable<LogEntry> entries)
        {
            var sum = 0.0;
            var count = 0;
            foreach (var e in entries)
                if (e.LatencyDetail != null && Math.Abs(e.LatencyDetail.Lag) > 0)
                {
                    count++;
                    sum += e.LatencyDetail.Lag;
                }
            if (Math.Abs(sum) <= 0 || count == 0) return 0;
            return sum / count;
        }

        private static double GetAverageRoundTripTime(IEnumerable<LogEntry> entries)
        {
            var sum = 0.0;
            var count = 0;
            foreach (var e in entries)
                if (e.LatencyDetail != null && Math.Abs(e.LatencyDetail.RoundTrip) > 0)
                {
                    count++;
                    sum += e.LatencyDetail.RoundTrip;
                }
            if (Math.Abs(sum) <= 0 || count == 0) return 0;
            return sum / count;
        }

        private bool _activeTest;
        private IEnumerator _currentTest;
        private readonly AsyncProcessor _asyncProcessor;
        private readonly List<LogEntry> _pings = new List<LogEntry>();
        private readonly List<LogEntry> _pongs = new List<LogEntry>();
        private readonly List<Action<PingTestResults>> _pingTestResultsListeners = new List<Action<PingTestResults>>();
    }
}