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
        public Button SendUnstructuredPacketBtn;
        
        public void Initialize(
            Action onLeaveSession,
            Action onSendTimePacket,
            Action<int> onSendBlankPacket,
            Action<int, int> onStartPingTest,
            Action<int> onSendUnstructuredPacket)
        {
            InitStartPingTestBtn(onStartPingTest);
            InitSendBlankPacketBtn(onSendBlankPacket);
            InitSendUnstructuredPacketBtn(onSendUnstructuredPacket);
            LeaveSessionBtn.onClick.AddListener(() => { onLeaveSession(); });
            SendTimePacketBtn.onClick.AddListener(() => { onSendTimePacket(); });
        }

        private void InitSendBlankPacketBtn(Action<int> onSendBlankPacket)
        {
            SendBlankPacketBtn.onClick.AddListener(() => { onSendBlankPacket(GetOpCode()); });
        }

        private void InitSendUnstructuredPacketBtn(Action<int> onSendUnstructuredPacket)
        {
            SendUnstructuredPacketBtn.onClick.AddListener(() => { onSendUnstructuredPacket(GetOpCode()); });
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

        private int GetOpCode()
        {
            int n;
            if (OpCode.text != string.Empty && int.TryParse(OpCode.text, out n) && n > 0) return n;
            OpCode.text = "1";
            return 1;
        }
    }
}