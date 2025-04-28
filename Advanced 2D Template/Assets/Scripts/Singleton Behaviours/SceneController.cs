using System.Collections;
using Types.Miscellaneous;
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

        private AnyGroup _sceneParameters;

        public void SetSOParameter(ScriptableObject so) => Instance.SetSceneParameters(anyGroup: new(Any.FromValue(so)));

        public void SetSceneParameters(AnyGroupAsset any) => Instance.SetSceneParameters(any.Value);

        public void SetSceneParameters(AnyGroup anyGroup)
        {
            _sceneParameters = anyGroup;
        }

        public T GetSceneParameter<T>(string key, T def = default)
        {
            if (_sceneParameters.TryGet(key, out T value))
                return value;

            return def;
        }

        public void Load(Types.Scene.SceneLoadSettingsAsset settings) => Instance.Load(settings.Value);

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

                if (EventSystem.current)
                    EventSystem.current.enabled = false;

                Instance.StartCoroutine(LoadWithTransition(settings));
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

        public void Reload()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Quit() => Application.Quit();
    }
}
