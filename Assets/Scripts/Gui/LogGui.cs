using UnityEngine;

namespace Gui
{
    public class LogGui : MonoBehaviour
    {
        public Transform LogContainer;
        public GameObject LogEntryPrefab;

        /**
         * <summaryClear Log</summary>
         */
        public void ClearLog()
        {
            for (var i = 0; i < LogContainer.childCount; i++) Destroy(LogContainer.GetChild(i).gameObject);
        }
        
        /**
         * <summary>Set Active</summary>
         */
        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
            if (state) return;
            ClearLog();
        }
    }
}