using UnityEngine;

namespace SingletonBehaviours
{
    public class ToastController : Types.SingletonBehaviour<ToastController>
    {
        [SerializeField] private GameObject _toastPrefab;
        private GameObject _currentToast;

        public void Spawn(string text)
        {
            if (_currentToast)
                Destroy(_currentToast);

            _currentToast = Instantiate(_toastPrefab, GameObject.FindGameObjectWithTag("Main Canvas").GetComponent<Canvas>().transform);

            _currentToast.transform.SetAsFirstSibling();
            _currentToast.GetComponentInChildren<TMPro.TMP_Text>().SetText(text);
        }
    }
}
