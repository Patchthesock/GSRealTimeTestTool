using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class RtSessionGui : MonoBehaviour
    {
        public Text SessionLog;
        public Button LeaveSessionBtn;

        public void Initialize(Action onLeaveSession)
        {
            LeaveSessionBtn.onClick.AddListener(() =>
            {
                SetActive(false);
                onLeaveSession();
            });
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
            if (!state) SessionLog.text = "";
        }

        public void OnPlayerConnect(int peerId)
        {
            SessionLog.text += "Player " + peerId + " has joined.\n";
        }

        public void OnPlayerDisconnect(int peerId)
        {
            SessionLog.text += "Player " + peerId + " has left.\n";
        }
    }
}