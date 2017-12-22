using System;
using System.Linq;
using System.Text;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using Gui;
using Models;

namespace Services
{
    public class MatchService
    {
        public MatchService(MatchGui matchGui)
        {
            _matchGui = matchGui;
        }
        
        public void Initialize(Action<RtSession> onStartRtSession)
        {
            InitialBtnState();
            MatchFoundMessage.Listener += OnMatchFound;
            MatchNotFoundMessage.Listener += OnMatchNotFound;
            _matchGui.Initialize(OnFindMatchReceived,
            () =>
            {
                InitialBtnState();
                SetActive(false);
                onStartRtSession(_rtSession);
                _matchGui.MatchDetails.text = "";
            });
        }
        
        /**
         * <summary>Set Active</summary>
         * <param name="state">State</param>
         **/
        public void SetActive(bool state)
        {
            _matchGui.gameObject.SetActive(state);
            if (state) return;
            _matchGui.SkillIpt.text = "";
            _matchGui.MatchNameIpt.text = "";
            _matchGui.MatchDetails.text = "";
        }
        
        private void OnFindMatchReceived(int skill, string shortCode)
        {
            _matchGui.MatchDetails.text = "Searching For Players...";
            new GameSparks.Api.Requests.MatchmakingRequest()
                .SetSkill(skill)    
                .SetMatchShortCode(shortCode)
                .Send(OnMatchMakingResponseReceived);
        }
        
        private void OnMatchFound(MatchFoundMessage m)
        {
            _matchGui.StartSessionBtn.gameObject.SetActive(true);
            _matchGui.MatchMakingBtn.gameObject.SetActive(false);
            _matchGui.MatchDetails.text = GetMatchInformation(m);
            _rtSession = new RtSession(m);
        }
        
        private void OnMatchNotFound(MatchNotFoundMessage message)
        {
            InitialBtnState();
            _matchGui.MatchDetails.text = "No Match Found...";
        }
        
        private void OnMatchMakingResponseReceived(MatchmakingResponse response)
        {
            if (response.HasErrors) _matchGui.MatchDetails.text = response.Errors.JSON;
        }
        
        private void InitialBtnState()
        {
            _matchGui.MatchMakingBtn.gameObject.SetActive(true);
            _matchGui.StartSessionBtn.gameObject.SetActive(false);
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

        private RtSession _rtSession;
        private readonly MatchGui _matchGui;
    }
}