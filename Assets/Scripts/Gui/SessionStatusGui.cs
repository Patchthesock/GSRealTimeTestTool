using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class SessionStatusGui : MonoBehaviour
    {
        public Text DisplayName;
        public Button EndSessionBtn;

        public void Initialize(Action onEndSessionRequestReceived)
        {
            InitLogoutBtn(onEndSessionRequestReceived);
        }
        
        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }
        
        private void InitLogoutBtn(Action onClick)
        {
            SetActive(false);
            EndSessionBtn.onClick.AddListener(() => { onClick(); });
        }
    }
}