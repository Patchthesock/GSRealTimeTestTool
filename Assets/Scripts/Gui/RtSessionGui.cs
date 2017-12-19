using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class RtSessionGui : MonoBehaviour
    {
        public Text SessionLog;
        public Button LeaveSessionBtn;

        /**
         * <summary>Intialize the Real Time Session Gui</summary>
         * <param name="onLeaveSession">Delegate Action, on leave session</param>
         **/
        public void Initialize(Action onLeaveSession)
        {
            LeaveSessionBtn.onClick.AddListener(() =>
            {
                SetActive(false);
                onLeaveSession();
            });
        }

        /**
         * <summary>Set RtSessionGui state</summary>
         * <param name="state">State to set RtSessionGui</param>
         **/
        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
            if (!state) SessionLog.text = "";
        }

        /**
         * <summary>On Player Connect</summary>
         * <param name="peerId">The player peerId of the connected player</param>
         **/
        public void OnPlayerConnect(int peerId)
        {
            SessionLog.text += "Player " + peerId + " has joined.\n";
        }

        /**
         * <summary>On Player Disconnect</summary>
         * <param name="peerId">The player peerId of the disconnected player</param>
         **/
        public void OnPlayerDisconnect(int peerId)
        {
            SessionLog.text += "Player " + peerId + " has left.\n";
        }
    }
}