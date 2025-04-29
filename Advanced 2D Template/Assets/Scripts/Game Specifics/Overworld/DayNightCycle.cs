using SingletonBehaviours;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCircle : MonoBehaviour
{
    [SerializeField] private Light2D _globalLight;

    [SerializeField] private Transform _lightParent;
    [SerializeField] private Light2D _sunLight;
    [SerializeField] private Light2D _moonLight;

    [SerializeField] private AnimationCurve _sunToMoonRatioOverTime;

    [SerializeField] private Gradient _skyColorOverTime;
    [SerializeField] private float _rotationSpeed;

    private void Update()
    {
        float scale = 1 / _rotationSpeed;
        SaveDataController.Instance.CurrentData.SetTime(SaveDataController.Instance.CurrentData.Time + (Time.deltaTime * scale));

        _globalLight.color = _skyColorOverTime.Evaluate(Mathf.Repeat(SaveDataController.Instance.CurrentData.Time, 1));

        Vector3 rotation = _lightParent.transform.rotation.eulerAngles;
        rotation.z = SaveDataController.Instance.CurrentData.Time * 360;
        _lightParent.transform.rotation = Quaternion.Euler(rotation);
        
        _sunLight.intensity = 1 - _sunToMoonRatioOverTime.Evaluate(Mathf.PingPong(SaveDataController.Instance.CurrentData.Time, 1));
        _moonLight.intensity = _sunToMoonRatioOverTime.Evaluate(Mathf.PingPong(SaveDataController.Instance.CurrentData.Time, 1));
    }
}
