using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class RealTimeControlGui : MonoBehaviour
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
            LeaveSessionBtn.onClick.AddListener(() => { onLeaveSession(); });
            SendTimePacketBtn.onClick.AddListener(() => { onSendTimePacket(); });
            SendBlankPacketBtn.onClick.AddListener(() => { onSendBlankPacket(GetOpCode()); });
            SendUnstructuredPacketBtn.onClick.AddListener(() => { onSendUnstructuredPacket(GetOpCode()); });
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
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