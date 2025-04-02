using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SingletonBehaviours
{
    public class ToastController : Types.SingletonBehaviour<ToastController>
    {
        [SerializeField] private GameObject _toastPrefab;

        public void PopupBottomLeft(string text)
        {
            var toast = Instantiate(_toastPrefab, FindFirstObjectByType<Canvas>().transform);
            toast.GetComponentInChildren<TMPro.TMP_Text>().SetText(text);
        }
    }
}
