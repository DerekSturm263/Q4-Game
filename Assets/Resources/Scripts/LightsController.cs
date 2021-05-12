using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightsController : MonoBehaviour
{
    private UIController uiCont;
    public Transform rotationLights;

    public Light2D skyLight;
    public Light2D sunLight;
    public Light2D moonLight;

    [SerializeField] private Color day;
    [SerializeField] private Color dusk;
    [SerializeField] private Color night;
    [SerializeField] private Color dawn;

    private void Awake()
    {
        uiCont = FindObjectOfType<UIController>();
    }

    private void Update()
    {
        rotationLights.rotation = uiCont.timeDisplay.transform.rotation;

        if (UIController.timeTitle == "day")
        {
            skyLight.color = day;

            sunLight.intensity = 1f;
            moonLight.intensity = 0f;
        }
        else if (UIController.timeTitle == "dusk")
        {
            skyLight.color = CustomLerp(day, dusk, night, 1 - (rotationLights.rotation.eulerAngles.z + 150f) % 30f / 29f);

            sunLight.intensity = Mathf.Lerp(0f, 1f, (rotationLights.rotation.eulerAngles.z - 150f) % 30f / 29f);
            moonLight.intensity = Mathf.Lerp(0.5f, 0f, (rotationLights.rotation.eulerAngles.z - 150f) % 30f / 29f);
        }
        else if(UIController.timeTitle == "night")
        {
            skyLight.color = night;

            sunLight.intensity = 0f;
            moonLight.intensity = 0.5f;
        }
        else
        {
            skyLight.color = CustomLerp(night, dawn, day, 1 - (rotationLights.rotation.eulerAngles.z + 30f) % 30f / 29f);

            sunLight.intensity = Mathf.Lerp(1f, 0f, (rotationLights.rotation.eulerAngles.z - 150f) % 30f / 29f);
            moonLight.intensity = Mathf.Lerp(0f, 0.5f, (rotationLights.rotation.eulerAngles.z - 150f) % 30f / 29f);
        }
    }

    private Color CustomLerp(Color a, Color b, Color c, float i)
    {
        if (i < 0.5f)
        {
            return Color.Lerp(a, b, i * 2f);
        }
        else
        {
            return Color.Lerp(b, c, (i - 0.5f) * 2f);
        }
    }
}
