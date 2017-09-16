using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Messages;
using GameSparks.Core;

namespace Gui
{
    public class MatchDetailsGui : MonoBehaviour
    {
        public Text MatchDetails;
        public Button MatchMakingBtn;
        public Button StartSessionBtn;
        public InputField SkillIpt;
        public InputField MatchNameIpt;

        public void Initialize(Action startSession, Action<int, string> findMatch)
        {
            InitialBtnState();
            MatchMakingBtn.onClick.AddListener(() =>
            {
                int d;
                if (!int.TryParse(SkillIpt.text, out d))
                {
                    SkillIpt.text = "0";
                    MatchDetails.text = "Invalid skill, >= 0";
                    return;
                }
                findMatch(d, MatchNameIpt.text);
                MatchDetails.text = "Searching For Players...";
            });
            StartSessionBtn.onClick.AddListener(() =>
            {
                InitialBtnState();
                SetActive(false);
                startSession();
                MatchDetails.text = "";
            });
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }
        
        public void OnMatchFound(MatchFoundMessage message)
        {
            StartSessionBtn.gameObject.SetActive(true);
            MatchMakingBtn.gameObject.SetActive(false);
            MatchDetails.text = GetMatchInformation(message);
        }

        public void OnMatchNotFound(MatchNotFoundMessage message)
        {
            InitialBtnState();
            MatchDetails.text = "No Match Found...";
        }

        public void OnMatchmakingError(GSData errors)
        {
            MatchDetails.text = errors.JSON;
        }

        private void InitialBtnState()
        {
            MatchMakingBtn.gameObject.SetActive(true);
            StartSessionBtn.gameObject.SetActive(false);
        }

        private static string GetMatchInformation(MatchFoundMessage m)
        {
            var s = new StringBuilder();
            s.AppendLine("Host URL:" + m.Host);
            s.AppendLine("Port:" + m.Port);
            s.AppendLine("Access Token:" + m.AccessToken);
            s.AppendLine("MatchId:" + m.MatchId);
            s.AppendLine("Opponents:" + m.Participants.Count());
            foreach (var p in m.Participants) s.AppendLine("Player: " + p.PeerId + " User Name: " + p.DisplayName);
            return s.ToString();
        }
    }
}