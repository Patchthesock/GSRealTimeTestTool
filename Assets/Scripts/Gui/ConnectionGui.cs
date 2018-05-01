using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class ConnectionGui : MonoBehaviour
    {
        public Text UserId;
        public Text ConnectionStatus;

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}