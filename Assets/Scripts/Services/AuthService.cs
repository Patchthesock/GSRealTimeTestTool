using System;
using GameSparks.Core;
using Gui;

namespace Services
{
    public class AuthService
    {
        public AuthService(AuthGui authGui)
        {
            _authGui = authGui;
        }

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
         **/
        public void SetActive(bool state)
        {
            _authGui.gameObject.SetActive(state);
        }

        private void EndSession(Action onEndSession)
        {
            onEndSession();
            _authGui.LogoutBtn.gameObject.SetActive(false);
            new GameSparks.Api.Requests.EndSessionRequest()
                .Send(endSessResponse =>
            {
                if (endSessResponse.HasErrors) _authGui.AddLogEntry(GetError(endSessResponse.Errors));
            });
        }
        
        private void DeviceAuthenticate(Action<string, string> onAuth)
        {
            new GameSparks.Api.Requests.DeviceAuthenticationRequest()
                .Send(r =>
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
            _authGui.ClearLog();
            onAuth(name, userId);
            _authGui.LogoutBtn.gameObject.SetActive(true);
        }
        
        private static string GetError(GSData error)
        {
            return error.JSON + "\n";
        }

        private readonly AuthGui _authGui;
    }
}