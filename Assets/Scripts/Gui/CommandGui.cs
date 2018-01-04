using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class CommandGui : MonoBehaviour
    {
        public InputField OpCode;
        public InputField Seconds;
        public InputField PacketsPerSecond;
        
        public Button LeaveSessionBtn;
        public Button SendTimePacketBtn;
        public Button SendBlankPacketBtn;
        public Button PacketThroughputTestBtn;
        
        public void Initialize(
            Action onLeaveSession,
            Action onSendTimePacket,
            Action<int> onSendBlankPacket,
            Action<int, int> onStartPingTest)
        {
            InitStartPingTestBtn(onStartPingTest);
            InitSendBlankPacketBtn(onSendBlankPacket);
            LeaveSessionBtn.onClick.AddListener(() => { onLeaveSession(); });
            SendTimePacketBtn.onClick.AddListener(() => { onSendTimePacket(); });
        }

        private void InitSendBlankPacketBtn(Action<int> onSendBlankPacket)
        {
            SendBlankPacketBtn.onClick.AddListener(() =>
            {
                int n;
                if (OpCode.text == string.Empty || !int.TryParse(OpCode.text, out n) || n <= 0)
                {
                    n = 1;
                    OpCode.text = "1";
                }
                onSendBlankPacket(n);
            });
        }

        private void InitStartPingTestBtn(Action<int, int> onStartPingTest)
        {
            PacketThroughputTestBtn.onClick.AddListener(() =>
            {
                int packetsPerSecond, seconds;
                if (Seconds.text == string.Empty || !int.TryParse(Seconds.text, out seconds) || seconds <= 0)
                {
                    seconds = 1;
                    Seconds.text = "1";
                }
                if (PacketsPerSecond.text == string.Empty || !int.TryParse(PacketsPerSecond.text, out packetsPerSecond)
                    || packetsPerSecond <= 0)
                {
                    packetsPerSecond = 1;
                    PacketsPerSecond.text = "1";
                }
                
                onStartPingTest(packetsPerSecond, seconds);
            });
        }
    }
}