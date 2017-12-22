using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class MatchGui : MonoBehaviour
    {
        public Text MatchDetails;
        public Button MatchMakingBtn;
        public Button StartSessionBtn;
        public InputField SkillIpt;
        public InputField MatchNameIpt;

        public void Initialize(Action<int, string> findMatchReceived, Action startSessionReceived)
        {
            StartSessionBtn.onClick.AddListener(() => { startSessionReceived(); });
            MatchMakingBtn.onClick.AddListener(() =>
            {
                int d;
                if (int.TryParse(SkillIpt.text, out d)) findMatchReceived(d, MatchNameIpt.text);
                else {
                    SkillIpt.text = "0";
                    MatchDetails.text = "Invalid skill, >= 0";
                }
            });
        }
    }
}