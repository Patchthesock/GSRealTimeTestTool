using System;
using System.Collections.Generic;
using GameSparks.Core;

namespace Gui.Service
{
    public class AuthGuiService
    {
        public AuthGuiService(
            AuthGui authGui,
            SessionStatusGui sessionGui)
        {
            _authGui = authGui;
            _sessionGui = sessionGui;
        }

        /**
         * <summary>Initialize</summary>
         * <param name="onAuth">On Player Authentication Logic</param>
         * <param name="onEndSession">On End Session Logic</param>
         */
        public void Initialize(
            Action onAuth,
            Action onEndSession)
        {
            _authGui.SetActive(false);
            _sessionGui.SetActive(false);
            _authGui.Initialize( // Initialize Authentication Screen
                () => { DeviceAuthenticate(onAuth); },
                (u, p) => { Authenticate(onAuth, u, p); });
            _sessionGui.Initialize(() => { EndSession(onEndSession); }); // Initialize Session Screen
            
            GS.GameSparksAvailable += r =>
            {
                if (!r) return;
                if (!GS.Authenticated) _authGui.SetActive(true);
                else
                {
                    OnAuthenticated();
                    onAuth();
                } 
            };
        }

        private void OnAuthenticated()
        {
            new GameSparks.Api.Requests.AccountDetailsRequest().Send(r =>
            {
                if (r.HasErrors) _authGui.AddLogEntry(GetError(r.Errors));
                else OnAuthenticated(r.UserId, r.DisplayName);
            });
        }

        private void OnAuthenticated(string userId, string username)
        {
            _authGui.ClearLog();
            _authGui.SetActive(false);
            _sessionGui.SetActive(true);
            _sessionGui.DisplayName.text = $"{username} ({userId})";
        }
        
        private void EndSession(Action onEndSession)
        {
            onEndSession();
            _authGui.SetActive(true);
            _sessionGui.SetActive(false);
            new GameSparks.Api.Requests.EndSessionRequest().Send(r =>
            {
                GS.Reset();
                if (r.HasErrors) _authGui.AddLogEntry(GetError(r.Errors));
            });
        }
        
        private void DeviceAuthenticate(Action onAuth)
        {
            new GameSparks.Api.Requests.DeviceAuthenticationRequest().Send(r =>
            {
                if (r.HasErrors) _authGui.AddLogEntry(GetError(r.Errors));
                else
                {
                    OnAuthenticated(r.UserId, r.DisplayName);
                    onAuth();
                }
            });
        }

        private void Authenticate(
            Action onAuth,
            string username,
            string password)
        {
            new GameSparks.Api.Requests.AuthenticationRequest()
                .SetUserName(username)
                .SetPassword(password)
                .Send(r =>
            {
                if (!r.HasErrors)
                {
                    OnAuthenticated(r.UserId, r.DisplayName);
                    onAuth();
                    return;
                }

                _authGui.AddLogEntry(GetError(r.Errors));
                if (string.Equals(GetError(r.Errors), "timeout")) return;
                new GameSparks.Api.Requests.RegistrationRequest()
                    .SetDisplayName(username)
                    .SetUserName(username)
                    .SetPassword(password)
                    .Send(r2 =>
                {
                    if (r2.HasErrors) _authGui.AddLogEntry(GetError(r2.Errors));
                    else
                    {
                        OnAuthenticated(r2.UserId, r2.DisplayName);
                        onAuth();
                    }
                });
            });
        }
        
        private static string GetError(GSData e)
        {
            var l = new List<string> {"DETAILS", "USERNAME"};
            foreach (var i in l)
            {
                if (e.GetString(i) == null) continue;
                switch (i)
                {
                    case "DETAILS": return "Incorrect Username & Password";
                    case "USERNAME": return "Username has already been registered";
                    default: return "Unknown Error";
                }
            }
            return string.Empty;
        }
        
        private readonly AuthGui _authGui;
        private readonly SessionStatusGui _sessionGui;
    }
}