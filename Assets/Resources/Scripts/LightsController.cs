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

        Color currentColor;
        Color nextColor;

        if (UIController.timeTitle == "day")
        {
            currentColor = day;
            nextColor = dusk;
        }
        else if (UIController.timeTitle == "dusk")
        {
            currentColor = dusk;
            nextColor = night;
        }
        else if(UIController.timeTitle == "night")
        {
            currentColor = night;
            nextColor = dawn;
        }
        else
        {
            currentColor = dawn;
            nextColor = day;
        }

        skyLight.color = Color.Lerp(currentColor, nextColor, rotationLights.rotation.eulerAngles.z % 90f / 89f);

        sunLight.intensity = Mathf.Lerp(0f, 1f, rotationLights.rotation.eulerAngles.z % 180);
        moonLight.intensity = Mathf.Lerp(1f, 0f, rotationLights.rotation.eulerAngles.z % 180);
    }
}
