﻿using System;
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
            LeaveSessionBtn.onClick.AddListener(() => { onLeaveSession(); });
            SendTimePacketBtn.onClick.AddListener(() => { onSendTimePacket(); });
            SendBlankPacketBtn.onClick.AddListener(() => { onSendBlankPacket(GetOpCode()); });
            SendUnstructuredPacketBtn.onClick.AddListener(() => { onSendUnstructuredPacket(GetOpCode()); });
            PacketThroughputTestBtn.onClick.AddListener(() => { onStartPingTest(GetPacketsPerSecond(), GetSeconds()); });
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        private int GetOpCode()
        {
            int n;
            if (OpCode.text != string.Empty && int.TryParse(OpCode.text, out n) && n > 0) return n;
            OpCode.text = "1";
            return 1;
        }

        private int GetSeconds()
        {
            int n;
            if (Seconds.text != string.Empty && int.TryParse(Seconds.text, out n) && n > 0) return n;
            Seconds.text = "1";
            return 1;
        }

        private int GetPacketsPerSecond()
        {
            int n;
            if (PacketsPerSecond.text != string.Empty && int.TryParse(PacketsPerSecond.text, out n) && n > 0) return n;
            PacketsPerSecond.text = "1";
            return 1;
        }
    }
}