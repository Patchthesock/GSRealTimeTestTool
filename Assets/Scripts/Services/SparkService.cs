using System;
using System.Collections.Generic;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.Core;

namespace Services
{
    public class SparkService
    {
        public void Initialize()
        {
            GS.GameSparksAvailable += OnGsAvailable;
            MatchFoundMessage.Listener += OnMatchFound;
            MatchNotFoundMessage.Listener += OnMatchNotFound;
        }
        
        public void FindMatch(string matchName, int skill, Action<MatchmakingResponse> onMatchResponse)
        {
            new GameSparks.Api.Requests.MatchmakingRequest()
                .SetMatchShortCode(matchName)
                .SetSkill(skill)
                .Send(onMatchResponse);
        }
        
        public void AuthenticateUser(string userName, string password, Action<AuthenticationResponse> onAuthenticatonResponse)
        {
            new GameSparks.Api.Requests.AuthenticationRequest()
                .SetUserName(userName)
                .SetPassword(password)
                .Send(onAuthenticatonResponse);
        }

        public void RegisterUser(string userName, string password, Action<RegistrationResponse> onRegistration)
        {
            new GameSparks.Api.Requests.RegistrationRequest()
                .SetDisplayName(userName)
                .SetUserName(userName)
                .SetPassword(password)
                .Send(onRegistration);
        }

        public void DeviceAuthenticateUser(Action<AuthenticationResponse> onAuthenticationResponse)
        {
            new GameSparks.Api.Requests.DeviceAuthenticationRequest()
                .Send(onAuthenticationResponse);
        }

        public void EndUserSession(Action<EndSessionResponse> onEndSessionResponse)
        {
            new GameSparks.Api.Requests.EndSessionRequest()
                .Send(onEndSessionResponse);
        }
        
        public void SubscribeOnGsAvailable(Action<bool> onGsAvailable)
        {
            if (_gsAvailableSubscribers.Contains(onGsAvailable)) return;
            _gsAvailableSubscribers.Add(onGsAvailable);
        }

        public void SubscribeOnMatchFound(Action<MatchFoundMessage> onMatchFound)
        {
            if (_matchFoundSubscribers.Contains(onMatchFound)) return;
            _matchFoundSubscribers.Add(onMatchFound);
        }

        public void SubscribeOnMatchNotFound(Action<MatchNotFoundMessage> onMatchNotFound)
        {
            if (_matchNotFoundSubscribers.Contains(onMatchNotFound)) return;
            _matchNotFoundSubscribers.Add(onMatchNotFound);
        }

        private void OnGsAvailable(bool isAvailable)
        {
            foreach (var l in _gsAvailableSubscribers) l(isAvailable);
        }

        private void OnMatchFound(MatchFoundMessage message)
        {
            foreach (var l in _matchFoundSubscribers) l(message);
        }

        private void OnMatchNotFound(MatchNotFoundMessage message)
        {
            foreach (var l in _matchNotFoundSubscribers) l(message);
        }

        private readonly List<Action<bool>> _gsAvailableSubscribers = new List<Action<bool>>();
        private readonly List<Action<MatchFoundMessage>> _matchFoundSubscribers = new List<Action<MatchFoundMessage>>();
        private readonly List<Action<MatchNotFoundMessage>> _matchNotFoundSubscribers = new List<Action<MatchNotFoundMessage>>();
    }
}