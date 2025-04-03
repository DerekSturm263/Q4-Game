using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    private Light2D _light;

    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private float _frequency;
    [SerializeField] private float _amplitude;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
    }

    private void Update()
    {
        _light.intensity = _curve.Evaluate(Mathf.Repeat(Time.time * _frequency, 1)) * _amplitude;
    }

    public void SetExcited()
    {
        _frequency = 4;
        _amplitude = 2;
    }

    public void SetDefault()
    {
        _frequency = 1;
        _amplitude = 1;
    }
}
