using System;
using GameSparks.Api.Responses;
using GameSparks.Core;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class AuthGui : MonoBehaviour
    {
        public Text Log;
        public Button LoginBtn;
        public Button LogoutBtn;
        public InputField UserNameInput;
        public InputField PasswordInput;
        public Button DeviceAuthenticationBtn;

        public void Initialize(
            SparkService sparkService,
            Action onEndSession,
            Action<RegistrationResponse> onReg,
            Action<AuthenticationResponse> onAuth)
        {
            InitDeviceAuthBtn(sparkService, onAuth);
            InitLogoutBtn(sparkService, onEndSession);
            InitLoginBtn(sparkService, onReg, onAuth);
        }
        
        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        private void InitLogoutBtn(SparkService sparkService, Action onEndSession)
        {
            LogoutBtn.gameObject.SetActive(false);
            LogoutBtn.onClick.AddListener(() =>
            {
                sparkService.EndUserSession(endSessResponse =>
                {
                    if (endSessResponse.HasErrors) Log.text = GetError(endSessResponse.Errors);
                    else
                    {
                        onEndSession();
                        LogoutBtn.gameObject.SetActive(false);
                    }
                });
            });
        }

        private void InitDeviceAuthBtn(SparkService sparkService, Action<AuthenticationResponse> onAuth)
        {
            DeviceAuthenticationBtn.onClick.AddListener(() =>
            {
                sparkService.DeviceAuthenticateUser(authResponse =>
                {
                    if (authResponse.HasErrors) Log.text = GetError(authResponse.Errors);
                    else
                    {
                        onAuth(authResponse);
                        LogoutBtn.gameObject.SetActive(true);
                    }
                });
            });
        }

        private void InitLoginBtn(
            SparkService sparkService,
            Action<RegistrationResponse> onReg,
            Action<AuthenticationResponse> onAuth)
        {
            LoginBtn.onClick.AddListener(() =>
            {
                sparkService.AuthenticateUser(UserNameInput.text, PasswordInput.text, authResponse =>
                {
                    if (!authResponse.HasErrors)
                    {
                        Log.text = "";
                        onAuth(authResponse);
                        LogoutBtn.gameObject.SetActive(true);
                    }
                    else
                    {
                        Log.text = GetError(authResponse.Errors);
                        sparkService.RegisterUser(UserNameInput.text, PasswordInput.text, regResponse =>
                        {
                            if (regResponse.HasErrors) Log.text += GetError(regResponse.Errors);
                            else
                            {
                                Log.text = "";
                                onReg(regResponse);
                                LogoutBtn.gameObject.SetActive(true);
                            }
                        });
                    }
                });
            });
        }

        private static string GetError(GSData error)
        {
            return error.JSON + "\n";
        }
    }
}