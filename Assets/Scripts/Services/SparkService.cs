using System;
using System.Collections.Generic;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.Core;

namespace Services
{
    public class SparkService
    {
        /**
         * <summary>Initialize the Spark Service</summary>
         **/
        public void Initialize()
        {
            GS.GameSparksAvailable += OnGsAvailable;
            MatchFoundMessage.Listener += OnMatchFound;
            MatchNotFoundMessage.Listener += OnMatchNotFound;
        }
        
        /**
         * <summary>Find Match</summary>
         * <param name="shortcode">The match shortcode to find</param>
         * <param name="skill">The skill of the player looking for a match</param>
         * <param name="onMatchResponse">Delegate Action with a MatchMakingResponse param</param>
         **/
        public void FindMatch(string shortcode, int skill, Action<MatchmakingResponse> onMatchResponse)
        {
            new GameSparks.Api.Requests.MatchmakingRequest()
                .SetMatchShortCode(shortcode)
                .SetSkill(skill)
                .Send(onMatchResponse);
        }
        
        /**
         * <summary>Authenticate a User</summary>
         * <param name="userName">User userName</param>
         * <param name="password">User password</param>
         * <param name="onAuthenticationResponse>Delegate Action with a AuthenticationResponse param</param>
         **/
        public void AuthenticateUser(string userName, string password, Action<AuthenticationResponse> onAuthenticatonResponse)
        {
            new GameSparks.Api.Requests.AuthenticationRequest()
                .SetUserName(userName)
                .SetPassword(password)
                .Send(onAuthenticatonResponse);
        }

        /**
         * <summary>Register a User</summary>
         * <param name="userName">User userName</param>
         * <param name="password">User password</param>
         * <param name="onRegistrationResponse">Delegate Action with a RegistrationResponse param</param>
         **/
        public void RegisterUser(string userName, string password, Action<RegistrationResponse> onRegistrationResponse)
        {
            new GameSparks.Api.Requests.RegistrationRequest()
                .SetDisplayName(userName)
                .SetUserName(userName)
                .SetPassword(password)
                .Send(onRegistrationResponse);
        }

        /**
         * <summary>Device Authenticate a User</summary>
         * <param name="onAuthenticationResponse">Delegate Action with a AuthenticationResponse param</param>
         **/
        public void DeviceAuthenticateUser(Action<AuthenticationResponse> onAuthenticationResponse)
        {
            new GameSparks.Api.Requests.DeviceAuthenticationRequest()
                .Send(onAuthenticationResponse);
        }

        /**
         * <summary>End User Session</summary>
         * <param name="onEndSessionResponse">Delgate Action with an EndSessionResponse param</param>
         **/
        public void EndUserSession(Action<EndSessionResponse> onEndSessionResponse)
        {
            new GameSparks.Api.Requests.EndSessionRequest()
                .Send(onEndSessionResponse);
        }
        
        #region Subscriptions
        
        /**
         * <summary>Subscribe To On GameSparks Available</summary>
         * <param name="onGsAvailable">Delegate Action with a bool state param</param>
         **/
        public void SubscribeOnGsAvailable(Action<bool> onGsAvailable)
        {
            if (_gsAvailableSubscribers.Contains(onGsAvailable)) return;
            _gsAvailableSubscribers.Add(onGsAvailable);
        }

        /**
         * <summary>Subscribe To On Match Found</summary>
         * <param name="onMatchFound>Delegate Action with a MatchFoundMessage param</param>
         **/
        public void SubscribeOnMatchFound(Action<MatchFoundMessage> onMatchFound)
        {
            if (_matchFoundSubscribers.Contains(onMatchFound)) return;
            _matchFoundSubscribers.Add(onMatchFound);
        }

        /**
         * <summary>Subscribe To On Match Not Found</summary>
         * <param name="onMatchNotFound">Delegate Action with a MatchNotFoundMessage param</param>
         **/
        public void SubscribeOnMatchNotFound(Action<MatchNotFoundMessage> onMatchNotFound)
        {
            if (_matchNotFoundSubscribers.Contains(onMatchNotFound)) return;
            _matchNotFoundSubscribers.Add(onMatchNotFound);
        }
        #endregion

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