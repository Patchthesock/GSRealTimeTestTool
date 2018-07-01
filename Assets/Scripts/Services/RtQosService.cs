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
        
        /**
         * <summary>Begin a Ping Test</summary>
         * <param name="seconds">How long to run the ping test for</param>
         * <param name="packetsPerSecond">How many packets to send per second of the ping test</param>
         * <param name="sendPing">The method of sending a ping</param>
         */
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
        
        /**
         * <summary>On Packet Log Entry Received</summary>
         * <param name="l">The Log Entry Received</param>
         */
        public void OnLogEntryReceived(ILogEntry l)
        {
            if (!_activeTest) return;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (l.GetLogEntryType())
            {
                case LogEntryTypes.PingPacket:
                {
                    _pingCount++;
                    return;
                }
                case LogEntryTypes.PongPacket:
                {
                    var i = (PongPacketLog) l;
                    _pongs.Add(i.GetLatency());
                    return;
                }
                default: return;
            }
        }

        /**
         * <summary>Subscribe to the ping test results</summary>
         * <param name="onPingTestResult">The delegate to receive the ping test result ILogEntry</param>
         */
        public void SubscribeToPingTestResults(Action<ILogEntry> onPingTestResult)
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
                GetAverageThroughput(_pongs),
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

        private static double GetAverageLatency(IEnumerable<Latency> e)
        {
            return GetAverage(e
                .Where(i => i.Lag > 0)
                .Select(i => i.Lag).ToList());
        }
        
        private static double GetAverageThroughput(IEnumerable<Latency> e)
        {
            return GetAverage(e
                .Where(i => i.Throughput > 0)
                .Select(i => i.Throughput).ToList());
        }

        private static double GetAverageRoundTripTime(IEnumerable<Latency> e)
        {
            return GetAverage(e
                .Where(i => i.RoundTrip > 0)
                .Select(i => i.RoundTrip).ToList());
        }

        private int _pingCount;
        private bool _activeTest;
        private IEnumerator _currentTest;
        
        private readonly AsyncProcessor _asyncProcessor;
        private readonly List<Latency> _pongs = new List<Latency>();
        private readonly List<Action<ILogEntry>> _pingTestResultsListeners = new List<Action<ILogEntry>>();
    }
}