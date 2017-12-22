using UnityEngine;

namespace Gui
{
    public class SessionGui : MonoBehaviour
    {
        public LogGui LogGui;
        public CommandGui CommandGui;
        public SessionDetailsGui SessionDetailsGui;
        public LogEntryInspectionGui LogEntryInspGui;

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
            LogGui.gameObject.SetActive(state);
            CommandGui.gameObject.SetActive(state);
            LogEntryInspGui.gameObject.SetActive(state);
            SessionDetailsGui.gameObject.SetActive(state);
            if (!state) LogEntryInspGui.Inspection.text = "";
        }
    }
}