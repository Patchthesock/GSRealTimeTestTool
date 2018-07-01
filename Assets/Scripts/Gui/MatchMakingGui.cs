using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class MatchMakingGui : MonoBehaviour
    {
        public Button MatchMakingBtn;
        public Button JoinRealTimeSessionBtn;
        
        public InputField SkillIpt;
        public InputField MatchShortCodeIpt;

        public Dropdown RealTimeSessionDropDown;

        /**
         * <summary>Initialize</summary>
         * <param name="onJoinSession"></param>
         * <param name="onFindMatch"></param>
         */
        public void Initialize(
            Action<string> onJoinSession,
            Action<int, string> onFindMatch)
        {
            SetupMatchMakingBtn(onFindMatch);
            SetupJoinRealTimeSessionBtn(onJoinSession);
            JoinRealTimeSessionBtn.interactable = false;
            RealTimeSessionDropDown.interactable = false;
        }

        /**
         * <summary>Set Active</summary>
         */
        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        /**
         * <summary>Clear Real Time Session Keys</summary>
         */
        public void ClearRealTimeSessionKeys()
        {
            RealTimeSessionDropDown.ClearOptions();
            JoinRealTimeSessionBtn.interactable = false;
            RealTimeSessionDropDown.interactable = false;
        }
        
        /**
         * <summary>Add Real Time Session Key</summary>
         * <param name="rtKey">The Real Time Session Key</param>
         */
        public void AddRealTimeSessionKey(string rtKey)
        {
            JoinRealTimeSessionBtn.interactable = true;
            RealTimeSessionDropDown.interactable = true;
            RealTimeSessionDropDown.AddOptions(new List<string> { rtKey });
        }

        private void SetupMatchMakingBtn(Action<int, string> onFindMatch)
        {
            MatchMakingBtn.onClick.AddListener(() =>
            {
                int d;
                if (!int.TryParse(SkillIpt.text, out d))
                {
                    d = 0;
                    SkillIpt.text = "0";
                }
                onFindMatch(d, MatchShortCodeIpt.text);
            });
        }

        private void SetupJoinRealTimeSessionBtn(Action<string> onJoinSession)
        {
            JoinRealTimeSessionBtn.onClick.AddListener(() =>
            {
                if (RealTimeSessionDropDown.options.Count <= 0) return;
                onJoinSession(RealTimeSessionDropDown.options[RealTimeSessionDropDown.value].text);
            });
        }
    }
}