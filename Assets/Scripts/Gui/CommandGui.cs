using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class CommandGui : MonoBehaviour
    {
        public InputField OpCode;
        public Button LeaveSessionBtn;
        public Button SendTimePacketBtn;
        public Button SendBlankPacketBtn;
        public Button PacketThroughputTestBtn;
        
        public void Initialize(
            Action onLeaveSession,
            Action onSendTimePacket,
            Action onPacketThroughput,
            Action<int> onSendBlankPacket)
        {
            InitSendBlankPacketBtn(onSendBlankPacket);
            LeaveSessionBtn.onClick.AddListener(() => { onLeaveSession(); });
            SendTimePacketBtn.onClick.AddListener(() => { onSendTimePacket(); });
            PacketThroughputTestBtn.onClick.AddListener(() => { onPacketThroughput(); });
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
    }
}