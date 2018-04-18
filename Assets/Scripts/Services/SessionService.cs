using System;
using Gui;
using Models;
using UnityEngine;

namespace Services
{
    public class SessionService
    {
        public SessionService(
            SessionGui sessionGui,
            PrefabBuilder prefabBuilder)
        {
            _sessionGui = sessionGui;
            _prefabBuilder = prefabBuilder;
        }

        public void Initialize(
            Action onSendPing,
            Action onStopRtSession,
            Action<int> onSendBlankPacket,
            Action<int, int> onStartPingTest,
            Action<int> onSendUnstructuredPacket)
        {
           _sessionGui.CommandGui.Initialize(
               onStopRtSession, onSendPing, onSendBlankPacket, onStartPingTest, onSendUnstructuredPacket);
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
         * <summary>On Ping Test Result Received</summary>
         * <param name="r">Ping Test Result</param>
         */
        public void OnPingTestResultReceived(PingTestResults r)
        {
            _sessionGui.SessionDetailsGui.Details.text = GetPingTestResultText(r);
        }
        
        /**
         * <summary>On Log Entry Received</summary>
         * <param name="l">Log Entry</param>
         */
        public void OnLogEntryReceived(LogEntry l)
        {
            var e = _prefabBuilder.Instantiate(_sessionGui.LogGui.LogEntryPrefab);
            e.transform.SetParent(_sessionGui.LogGui.LogContainer);
            e.transform.localScale = new Vector3(1, 1, 1);
            e.GetComponent<LogEntryBtn>().Initialize(GetEntryTitle(l), () =>
            {
                _sessionGui.LogEntryInspGui.Inspection.text = GetInspectionText(l);
            });
        }

        private static string GetEntryTitle(LogEntry l)
        {
            return l.Message + " :: " + l.Direction + " :: " + l.CreatedAt;
        }

        private static string GetInspectionText(LogEntry l)
        {
            var s = l.Message + "\nDirection: " + l.Direction + "\n\n";
            if (l.PacketDetail != null) s += GetPacketDetails(l.PacketDetail);
            if (l.LatencyDetail != null) s += GetLatencyDetails(l.LatencyDetail);
            return s;
        }
        
        private static string GetPacketDetails(PacketDetails d)
        {
            var s = "OpCode: " + d.OpCode + "\n";
            s += "Packet Size: " + d.Size + "\n";
            s += "Sender: Peer " + d.Sender + "\n";
            s += "Request ID: " + d.RequestId + "\n";
            s += "Stream: " + d.Stream + "\n";
            return s;
        }

        private static string GetLatencyDetails(Latency l)
        {
            var s = "Speed: " + l.Speed + " kbit/s\n";
            s += "Latency: " + l.Lag + "\n";
            s += "Round Trip Time: " + l.RoundTrip + "\n\n";
            return s;
        }

        private static string GetPingTestResultText(PingTestResults r)
        {
            var s = "Pings Sent: " + r.PingsSent + "\n";
            s += "Pongs Received: " + r.PongsReceived + "\n";
            s += "Average kbits: " + r.AverageKBits + "\n";
            s += "Average Latency: " + r.AverageLatency + "\n";
            s += "Average Round Trip Time: " + r.AverageRoundTripTime + "\n";
            return s;
        }

        private readonly SessionGui _sessionGui;
        private readonly PrefabBuilder _prefabBuilder;
    }
}