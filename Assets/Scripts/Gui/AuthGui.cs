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

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        private static string GetError(GSData error)
        {
            return error.JSON + "\n";
        }
    }
}