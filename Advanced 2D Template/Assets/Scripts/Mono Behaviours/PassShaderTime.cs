using UnityEngine;
using UnityEngine.UI;

namespace MonoBehaviours
{
    public class PassShaderTime : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private string _parameterName;
        [SerializeField] private float _scale;

        private float _timePassed;

        private void Awake()
        {
            _timePassed = 0;
        }

        private void Update()
        {
            _timePassed += Time.deltaTime * _scale;

            _image.material.SetFloat(_parameterName, _timePassed);
        }
    }
}
