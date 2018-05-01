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

        public void Initialize(
            Action<string> joinSession,
            Action<int, string> findMatch)
        {
            SetupMatchMakingBtn(findMatch);
            SetupJoinRealTimeSessionBtn(joinSession);
        }

        /**
         * <summary>Set Active</summary>
         */
        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        /**
         * <summary>Add Real Time Session Key</summary>
         */
        public void AddRealTimeSessionKey(string rtKey)
        {
            RealTimeSessionDropDown.AddOptions(new List<string> { rtKey });
        }

        private void SetupMatchMakingBtn(Action<int, string> findMatch)
        {
            MatchMakingBtn.onClick.AddListener(() =>
            {
                int d;
                if (int.TryParse(SkillIpt.text, out d)) findMatch(d, MatchShortCodeIpt.text);
                else {
                    SkillIpt.text = "0";
                    findMatch(0, MatchShortCodeIpt.text);
                }
            });
        }

        private void SetupJoinRealTimeSessionBtn(Action<string> joinSession)
        {
            JoinRealTimeSessionBtn.onClick.AddListener(() =>
            {
                joinSession(RealTimeSessionDropDown.options[RealTimeSessionDropDown.value].text);
            });
        }
    }
}