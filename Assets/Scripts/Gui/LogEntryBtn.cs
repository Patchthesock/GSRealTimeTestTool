using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    [RequireComponent(typeof(Button))]
    public class LogEntryBtn : MonoBehaviour
    {
        public void Initialize(string title, Action onClick)
        {
            GetComponentInChildren<Text>().text = title;
            GetComponent<Button>().onClick.AddListener(() => { onClick(); });
        }
    }
}