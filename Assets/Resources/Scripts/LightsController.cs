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

    public static float targetStrength = 1f;
    private static float strength = 1f;

    [SerializeField] private Color dawnColorUI;
    [SerializeField] private Color dayColorUI;
    [SerializeField] private Color duskColorUI;
    [SerializeField] private Color nightColorUI;

    private Color newColor;

    public UnityEngine.UI.Image uiColor;

    private void Awake()
    {
        uiCont = FindObjectOfType<UIController>();
    }

    private void Update()
    {
        strength = Mathf.Lerp(strength, targetStrength, Time.deltaTime * 5f);
        rotationLights.rotation = uiCont.timeDisplay.transform.rotation;
        skyLight.intensity = 0.9f * strength;

        if (UIController.timeTitle == "day")
        {
            skyLight.color = day;
            newColor = dayColorUI;

            sunLight.intensity = 1f * strength;
            moonLight.intensity = 0f * strength;
        }
        else if (UIController.timeTitle == "dusk")
        {
            skyLight.color = CustomLerp(day, dusk, night, 1 - (rotationLights.rotation.eulerAngles.z + 150f) % 30f / 29f);
            newColor = duskColorUI;

            sunLight.intensity = Mathf.Lerp(0f, 1f, (rotationLights.rotation.eulerAngles.z - 150f) % 30f / 29f) * strength;
            moonLight.intensity = Mathf.Lerp(0.5f, 0f, (rotationLights.rotation.eulerAngles.z - 150f) % 30f / 29f) * strength;
        }
        else if(UIController.timeTitle == "night")
        {
            skyLight.color = night;
            newColor = nightColorUI;

            sunLight.intensity = 0f * strength;
            moonLight.intensity = 0.5f * strength;
        }
        else
        {
            skyLight.color = CustomLerp(night, dawn, day, 1 - (rotationLights.rotation.eulerAngles.z + 30f) % 30f / 29f);
            newColor = dawnColorUI;

            sunLight.intensity = Mathf.Lerp(1f, 0f, (rotationLights.rotation.eulerAngles.z - 150f) % 30f / 29f) * strength;
            moonLight.intensity = Mathf.Lerp(0f, 0.5f, (rotationLights.rotation.eulerAngles.z - 150f) % 30f / 29f) * strength;
        }

        uiColor.color = Color.Lerp(uiColor.color, newColor, Time.deltaTime);
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
