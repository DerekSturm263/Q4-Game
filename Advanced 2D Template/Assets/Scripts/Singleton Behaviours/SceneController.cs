using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace SingletonBehaviours
{
    public class SceneController : Types.SingletonBehaviour<SceneController>
    {
        [SerializeField] private GameObject _transitionPrefab;

        private Types.Scene.SceneLoadSettings _last;
        private bool _isTransitioning;

        private GameObject _transitionCanvas;
        private GameObject _transitionInstance;

        private Types.Collections.Dictionary<string, Types.Miscellaneous.Any> _sceneParameters = new();
        public void SetSceneParameter<T>(string key, T value)
        {
            if (_sceneParameters.ContainsKey(key))
                _sceneParameters[key] = Types.Miscellaneous.Any.FromValue(value);
            else
                _sceneParameters.TryAdd(key, Types.Miscellaneous.Any.FromValue(value));
        }
        public T GetSceneParameter<T>(string key) => (T)(object)_sceneParameters[key];

        public void Load(Types.Scene.SceneLoadSettingsAsset settings) => Load(settings.Value);

        public void Load(Types.Scene.SceneLoadSettings settings)
        {
            if (_isTransitioning)
                return;

            if (settings.Transition)
            {
                foreach (MonoBehaviours.Input.InputEvent inputEvent in FindObjectsByType<MonoBehaviours.Input.InputEvent>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    inputEvent.enabled = false;
                }

                EventSystem.current.enabled = false;

                StartCoroutine(LoadWithTransition(settings));
            }
            else
            {
                LoadNoTransition(settings);
            }
        }

        public void LoadLast(Types.Scene.SceneLoadSettingsAsset fallback) => Load(_last.Scene.Name == "" ? fallback.Value : _last);

        private void StartTransition(Types.Scene.SceneLoadSettings settings)
        {
            if (!_transitionCanvas)
                _transitionCanvas = GameObject.FindGameObjectWithTag("Transition Canvas");

            _transitionInstance = Instantiate(_transitionPrefab, _transitionCanvas.transform);
            _transitionInstance.GetComponent<Animator>().runtimeAnimatorController = settings.Transition;

            _isTransitioning = true;
        }

        private IEnumerator LoadWithTransition(Types.Scene.SceneLoadSettings settings)
        {
            StartTransition(settings);

            AsyncOperation operation = SceneManager.LoadSceneAsync(settings.Scene.Name, settings.LoadParameters);
            operation.allowSceneActivation = false;

            yield return new WaitUntil(() => operation.progress >= 0.9f);

            if (settings.SceneParameters.HasValue)
                _sceneParameters = settings.SceneParameters.Value;

            operation.allowSceneActivation = true;

            StopTransition(settings);
        }

        private void StopTransition(Types.Scene.SceneLoadSettings settings)
        {
            if (_transitionInstance)
                _transitionInstance.GetComponent<Animator>().SetTrigger("Finished");

            _isTransitioning = false;
            _last = settings;
        }

        private void LoadNoTransition(Types.Scene.SceneLoadSettings settings)
        {
            SceneManager.LoadScene(settings.Scene.Name);
        }

        public void Quit() => Application.Quit();
    }
}
