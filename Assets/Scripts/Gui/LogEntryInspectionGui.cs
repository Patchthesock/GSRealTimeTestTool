using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class LogEntryInspectionGui : MonoBehaviour
    {
        public Text Inspection;

        public void SetInspectionText(string t)
        {
            Inspection.text = t;
        }
    }
}