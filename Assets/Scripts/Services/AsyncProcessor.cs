using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Services
{
    public class AsyncProcessor : MonoBehaviour
    {
        public void Start()
        {
            _eventSystem = EventSystem.current;
        }

        public void Update()
        {
            // Hack for InputField Tab Navigation
            if (!Input.GetKeyDown(KeyCode.Tab)) return;
            var next = _eventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next == null) return;
            var field = next.GetComponent<InputField>();
            if (field == null) return;
            field.OnPointerClick(new PointerEventData(_eventSystem));
            _eventSystem.SetSelectedGameObject(next.gameObject, new BaseEventData(_eventSystem));
        }
        
        private EventSystem _eventSystem;
    }
}