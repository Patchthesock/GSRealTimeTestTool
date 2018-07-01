using System;
using System.Collections.Generic;
using GameSparks.Api.Messages;
using Models;

namespace Gui.Service
{
    public class MatchService
    {
        public void Initialize()
        {
            MatchFoundMessage.Listener += OnMatchFound;
            MatchUpdatedMessage.Listener += OnMatchUpdated;
            MatchNotFoundMessage.Listener += OnMatchNotFound;
        }
        
        public void FindMatch(int skill, string shortCode, Action<string> onError)
        {
            new GameSparks.Api.Requests.MatchmakingRequest()
                .SetSkill(skill)    
                .SetMatchShortCode(shortCode)
                .Send(res =>
                {
                    if (!res.HasErrors) return;
                    switch (res.Errors.JSON.ToString())
                    {
                        case "{\"matchShortCode\":\"NOT_FOUND\"}":
                            onError("Match Type/Shortcode Not Found");
                            break;
                        default:
                            onError("Unknown Error");
                            break;
                    }
                });
        }

        public void SubscribeToOnMatchFound(Action<RtSession> onMatchFound)
        {
            if (_onMatchFoundListeners.Contains(onMatchFound)) return;
            _onMatchFoundListeners.Add(onMatchFound);
        }

        public void SubscribeToOnMatchNotFound(Action onMatchNotFound)
        {
            if (_onMatchNotFoundListeners.Contains(onMatchNotFound)) return;
            _onMatchNotFoundListeners.Add(onMatchNotFound);
        }
        
        private void OnMatchFound(MatchFoundMessage m)
        {
            var s = new RtSession(m);
            foreach (var l in _onMatchFoundListeners) l(s);
        }

        private void OnMatchUpdated(MatchUpdatedMessage m)
        {
            
        }
        
        private void OnMatchNotFound(MatchNotFoundMessage m)
        {
            foreach (var l in _onMatchNotFoundListeners) l();
        }

        private readonly List<Action> _onMatchNotFoundListeners = new List<Action>();
        private readonly List<Action<RtSession>> _onMatchFoundListeners = new List<Action<RtSession>>();
    }
}