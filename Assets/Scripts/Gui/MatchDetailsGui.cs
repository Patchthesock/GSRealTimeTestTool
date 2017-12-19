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

        /**
         * <summary>Initialize MatchDetailsGui</summary>
         * <param name="startSession">Delegate Action</param>
         * <param name="findMatch">Delegate Action with int(Skill) and string(MatchShortCode) param</param>
         **/
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

        /**
         * <summary>Set Match Details Gui Active</summary>
         * <param name="state">Match Details Gui active state</param>
         **/
        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }
        
        /**
         * <summary>Displays the details of the match found</summary>
         * <param name="m">MatchFoundMessage</param>
         **/
        public void OnMatchFoundReceived(MatchFoundMessage m)
        {
            StartSessionBtn.gameObject.SetActive(true);
            MatchMakingBtn.gameObject.SetActive(false);
            MatchDetails.text = GetMatchInformation(m);
        }

        /**
         * <summary>Displays No Match Found</summary>
         * <param name="m">MatchNotFoundMessage</param>
         **/
        public void OnMatchNotFoundReceived(MatchNotFoundMessage m)
        {
            InitialBtnState();
            MatchDetails.text = "No Match Found...";
        }

        /**
         * <summary>On Matchmaking Error Received, displays error</summary>
         * <param name="errors">GSData from MatchmakingResponse</param>
         **/
        public void OnMatchmakingErrorReceived(GSData errors)
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