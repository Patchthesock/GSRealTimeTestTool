using UnityEngine;

namespace Gui
{
    public class SessionGui : MonoBehaviour
    {
        public LogGui LogGui;
        public MatchMakingGui MatchMakingGui;
        public LogEntryInspectionGui LogEntryInspGui;
        public RealTimeControlGui RealTimeControlGui;
        
        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}