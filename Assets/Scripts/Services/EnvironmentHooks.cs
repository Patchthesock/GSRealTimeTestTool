using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class EnvironmentHooks : MonoBehaviour
    {
        public void SubscribeToOnApplicationFocusChange(Action<bool> onAppFocus)
        {
            if (!_onAppFocusListeners.Contains(onAppFocus)) return;
            _onAppFocusListeners.Add(onAppFocus);
        }

        public void SubscribeToOnApplicationPauseChange(Action<bool> onAppPause)
        {
            if (!_onAppPauseListeners.Contains(onAppPause)) return;
            _onAppPauseListeners.Add(onAppPause);
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            foreach (var l in _onAppFocusListeners) l(hasFocus);
        }
    
        private void OnApplicationPause(bool pauseStatus)
        {
            Debug.Log("Fired");
            foreach (var l in _onAppPauseListeners) l(pauseStatus);
        }

        private readonly List<Action<bool>> _onAppFocusListeners = new List<Action<bool>>();
        private readonly List<Action<bool>> _onAppPauseListeners = new List<Action<bool>>();
    }
}