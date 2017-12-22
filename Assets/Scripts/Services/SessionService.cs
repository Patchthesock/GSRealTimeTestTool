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
            Action onStopRtSession,
            Action onSendTimestampPacket,
            Action<int> onSendBlankPacket)
        {
           _sessionGui.CommandGui.Initialize(onStopRtSession, onSendTimestampPacket, () => { }, onSendBlankPacket);
        }
        
        /**
         * <summary>Set Active</summary>
         * <param name="state">State</param>
         **/
        public void SetActive(bool state)
        {   
            _sessionGui.SetActive(state);
        }

        /**
         * <summary>On Log Entry Received</summary>
         * <param name="l">Log Entry</param>
         **/
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
            return l.Message + " " + l.Direction;
        }

        private static string GetInspectionText(LogEntry l)
        {
            var s = l.Message + "\nDirection: " + l.Direction + "\n\n";
            if (l.PacketDetail != null) s += GetPacketDetails(l.PacketDetail);
            if (l.LatencyDetail != null) s += GetLatencyDetails(l.LatencyDetail);
            return s;
        }
        
        private static string GetPacketDetails(PacketDetails details)
        {
            var s = "OpCode: " + details.OpCode + "\n";
            s += "Packet Size: " + details.Size + "\n";
            s += "Sender: Player " + details.Sender + "\n";
            s += "Stream Length: " + details.StreamLength + "\n\n";
            return s;
        }

        private static string GetLatencyDetails(Latency latency)
        {
            var s = "Ping Time: " + latency.PingTime + "\n";
            s += "Pong Time: " + latency.PongTime + "\n";
            s += "Current Time: " + latency.CurrentTime + "\n";
            s += "Latency: " + latency.Lag + "\n";
            s += "Round Trip Time: " + latency.RoundTrip + "\n\n";
            return s;
        }

        private readonly SessionGui _sessionGui;
        private readonly PrefabBuilder _prefabBuilder;
    }
}