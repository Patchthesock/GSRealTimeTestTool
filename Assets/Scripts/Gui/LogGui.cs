using UnityEngine;

namespace Gui
{
    public class LogGui : MonoBehaviour
    {
        public Transform LogContainer;
        public GameObject LogEntryPrefab;

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
            if (state) return;
            for (var i = 0; i < LogContainer.childCount; i++) Destroy(LogContainer.GetChild(i).gameObject);
        }
    }
}