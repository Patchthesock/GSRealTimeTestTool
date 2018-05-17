using System;
using GameSparks.Core;

namespace Gui.Service
{
    public class AuthGuiService
    {
        public AuthGuiService(AuthGui authGui)
        {
            _authGui = authGui;
        }

        /**
         * <summary>Initialize</summary>
         * <param name="onEndSession">On End Session Logic</param>
         * <param name="onReg">On Player Registration Logic</param>
         * <param name="onAuth">On Player Authentication Logic</param>
         * <param name="onDeviceAuth">On Device Authentication Logic</param>
         */
        public void Initialize(
            Action onEndSession,
            Action<string, string> onReg,
            Action<string, string> onAuth,
            Action<string, string> onDeviceAuth)
        {
            _authGui.Initialize(
                () => { EndSession(onEndSession); },
                () => { DeviceAuthenticate(onDeviceAuth); },
                (u, p) => { Authenticate(u, p, onReg, onAuth); });
        }

        /**
         * <summary>Set Active</summary>
         * <param name="state">State</param>
         */
        public void SetActive(bool state)
        {
            _authGui.SetActive(state);
        }

        private void EndSession(Action onEndSession)
        {
            onEndSession();
            _authGui.SetAuthenticated(false);
            new GameSparks.Api.Requests.EndSessionRequest().Send(r =>
            {
                if (r.HasErrors) _authGui.AddLogEntry(GetError(r.Errors));
            });
        }
        
        private void DeviceAuthenticate(Action<string, string> onAuth)
        {
            new GameSparks.Api.Requests.DeviceAuthenticationRequest().Send(r =>
            {
                if (r.HasErrors) _authGui.AddLogEntry(GetError(r.Errors));
                else
                {
                    onAuth(r.DisplayName, r.UserId);
                    _authGui.LogoutBtn.gameObject.SetActive(true);
                }
            });
        }

        private void Authenticate(
            string username,
            string password,
            Action<string, string> onReg,
            Action<string, string> onAuth)
        {
            new GameSparks.Api.Requests.AuthenticationRequest()
                .SetUserName(username)
                .SetPassword(password)
                .Send(r =>
            {
                // TODO: Check if error is timeout related; if so don't do registration
                if (!r.HasErrors) OnAuth(r.DisplayName, r.UserId, onAuth);
                else
                {
                    _authGui.AddLogEntry(GetError(r.Errors));
                    
                    new GameSparks.Api.Requests.RegistrationRequest()
                        .SetDisplayName(username)
                        .SetUserName(username)
                        .SetPassword(password)
                        .Send(r2 =>
                    {
                        if (r2.HasErrors) _authGui.AddLogEntry(GetError(r2.Errors));
                        else OnAuth(r2.DisplayName, r2.UserId, onReg);
                    });
                }
            });
        }

        private void OnAuth(string name, string userId, Action<string, string> onAuth)
        {
            onAuth(name, userId);
            _authGui.SetAuthenticated(true);
        }
        
        private static string GetError(GSData error)
        {
            return error.JSON + "\n";
        }

        private readonly AuthGui _authGui;
    }
}