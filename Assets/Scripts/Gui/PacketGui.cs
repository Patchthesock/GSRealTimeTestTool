using System;
using Models;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class PacketGui : MonoBehaviour
    {
        public Text Log;
        public InputField OpCode;
        public Button SendTimePacketBtn;
        public Button SendBlankPacketBtn;
        
        public void Initialize(Action onSendTimePacket, Action<int> onSendBlankPacket)
        {
            InitSendTimePacketBtn(onSendTimePacket);
            InitSendBlankPacketBtn(onSendBlankPacket);
        }
        
        public void OnBlackPacketARecieved(PacketDetails details)
        {
            Log.text = GetPacketDetails(details);
        }
        
        public void OnLatencyReceived(Latency latency, PacketDetails details)
        {
            Log.text = GetLatencyDetails(latency);
            Log.text += GetPacketDetails(details);
        }

        private void InitSendTimePacketBtn(Action onSendTimePacket)
        {
            SendTimePacketBtn.onClick.AddListener(() =>
            {
                Log.text = "Sending...\n";
                onSendTimePacket();
                Log.text += "Sent...\n";
            });
        }

        private void InitSendBlankPacketBtn(Action<int> onSendBlankPacket)
        {
            SendBlankPacketBtn.onClick.AddListener(() =>
            {
                int n;
                if (OpCode.text == string.Empty || !int.TryParse(OpCode.text, out n) || n <= 0)
                {
                    Log.text = "Please enter an OpCode e.g. 1 - 1000\n";
                    return;
                }
                Log.text = "Sending...\n";
                onSendBlankPacket(n);
                Log.text += "Sent...\n";
            });
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
            s += "Curent Time: " + latency.CurrentTime + "\n";
            s += "Latency: " + latency.Lag + "\n";
            s += "Round Trip: " + latency.RoundTrip + "\n\n";
            return s;
        }
    }
}