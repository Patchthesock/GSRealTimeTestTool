using System;
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
            Action onEndSessionRequestReceived,
            Action onDeviceAuthenticationRequestReceived,
            Action<string, string> onAuthenticationRequestReceived)
        {
            ClearLog();
            InitLogoutBtn(onEndSessionRequestReceived);
            InitLoginBtn(onAuthenticationRequestReceived);
            InitDeviceAuthBtn(onDeviceAuthenticationRequestReceived);
        }

        /**
         * <summary>Clear Log</summary>
         **/
        public void ClearLog()
        {
            Log.text = "";
        }
        
        /**
         * <summary>Add Log Entry</summary>
         * <param name="msg">The log message to add</param>
         **/
        public void AddLogEntry(string msg)
        {
            Log.text += msg + "\n";
        }
        
        private void InitLogoutBtn(Action onClick)
        {
            LogoutBtn.gameObject.SetActive(false);
            LogoutBtn.onClick.AddListener(() => { onClick(); });
        }

        private void InitDeviceAuthBtn(Action onClick)
        {
            DeviceAuthenticationBtn.onClick.AddListener(() => { onClick(); });
        }

        private void InitLoginBtn(Action<string, string> onClick)
        {
            LoginBtn.onClick.AddListener(() => { onClick(UserNameInput.text, PasswordInput.text); });
        }
    }
}