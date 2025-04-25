using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Types.Scene
{
    [Serializable]
    public struct SceneLoadSettings
    {
        [SerializeField] private SceneField _scene;
        public readonly SceneField Scene => _scene;

        [SerializeField] private RuntimeAnimatorController _transition;
        public readonly RuntimeAnimatorController Transition => _transition;

        [SerializeField] private LoadSceneParameters _loadParameters;
        public readonly LoadSceneParameters LoadParameters => _loadParameters;
    }
}
