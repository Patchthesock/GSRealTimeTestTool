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
                    onAuth();
                    OnAuthenticated();
                } 
            };
        }

        private void OnAuthenticated()
        {
            new GameSparks.Api.Requests.AccountDetailsRequest().Send(r =>
            {
                if (!r.HasErrors) OnAuthenticated(r.UserId, r.DisplayName);
                else _authGui.AddLogEntry(GetError(r.Errors, _authErrors).Description);
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
                if (r.HasErrors) _authGui.AddLogEntry(GetError(r.Errors, _authErrors).Description);
            });
        }
        
        private void DeviceAuthenticate(Action onAuth)
        {
            new GameSparks.Api.Requests.DeviceAuthenticationRequest().SetDurable(true).Send(r =>
            {
                if (r.HasErrors) _authGui.AddLogEntry(GetError(r.Errors, _authErrors).Description);
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
            _authErrors.Clear();
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

                // If it's a timeout issue return
                _authGui.AddLogEntry(GetError(r.Errors, _authErrors).Description);
                if (string.Equals(GetError(r.Errors, _authErrors).Id, "TIMEOUT")) return;
                
                _authGui.AddLogEntry("Attempting Registration");
                new GameSparks.Api.Requests.RegistrationRequest()
                    .SetDisplayName(username)
                    .SetUserName(username)
                    .SetPassword(password)
                    .Send(r2 =>
                {
                    if (r2.HasErrors) _authGui.AddLogEntry(GetError(r2.Errors, _authErrors).Description);
                    else
                    {
                        OnAuthenticated(r2.UserId, r2.DisplayName);
                        onAuth();
                    }
                });
            });
        }
        
        private static Error GetError(GSData e, IEnumerable<Error> errors)
        {
            foreach (var i in errors)
            {
                if (e.GetString(i.Id) == null) continue;
                return i;
            }
            return new Error("UNKNOWN", "Unknown Error");
        }
        
        private readonly AuthGui _authGui;
        private readonly SessionStatusGui _sessionGui;

        private readonly List<Error> _authErrors = new List<Error>
        {
            new Error("TIMEOUT", "Timeout occured"),
            new Error("DETAILS", "Incorrect Username &/or Password"),
            new Error("USERNAME", "Username has already been registered")
        };

        private class Error
        {
            public readonly string Id;
            public readonly string Description;

            public Error(string id, string description)
            {
                Id = id;
                Description = description;
            }
        }
    }
}