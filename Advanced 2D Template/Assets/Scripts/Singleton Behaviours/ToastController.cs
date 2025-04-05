using UnityEngine;

namespace SingletonBehaviours
{
    public class ToastController : Types.SingletonBehaviour<ToastController>
    {
        [SerializeField] private GameObject _toastPrefab;
        private GameObject _currentToast;

        public void PopupBottomLeft(string text)
        {
            if (_currentToast)
                Destroy(_currentToast);

            _currentToast = Instantiate(_toastPrefab, FindFirstObjectByType<Canvas>().transform);
            _currentToast.GetComponentInChildren<TMPro.TMP_Text>().SetText(text);
        }
    }
}
