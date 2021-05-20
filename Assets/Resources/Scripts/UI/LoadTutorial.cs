using UnityEngine;
using UnityEngine.InputSystem;

public class LoadTutorial : MonoBehaviour
{
    public GameObject tutorialRef;

    [SerializeField] private TMPro.TMP_Text labelTextRef;
    [SerializeField] private TMPro.TMP_Text descriptionTextRef;
    [SerializeField] private TMPro.TMP_Text continueTextRef;

    public static GameObject tutorial;
    public static Animator anim;

    private static TMPro.TMP_Text labelText;
    private static TMPro.TMP_Text descriptionText;
    private static TMPro.TMP_Text continueText;

    private void Awake()
    {
        tutorial = tutorialRef;
        anim = tutorial.GetComponent<Animator>();

        labelText = labelTextRef;
        descriptionText = descriptionTextRef;
        continueText = continueTextRef;

        // Make the continue text match the name of the device that's being used.
        InputSystem.onDeviceChange += (device, change) =>
        {
            ChangeContinueText(PlayerMovement.controls.UI.Select.name);
        };

        ChangeContinueText("SPACE");
        Abilities.Initialize();
        tutorial.SetActive(false);
    }

    public static void Display(string label, string description)
    {
        tutorial.SetActive(true);

        labelText.text = label;
        descriptionText.text = description;

        PlayerMovement.Freeze();
    }

    public static void Display(AbilityTutorial ability)
    {
        tutorial.SetActive(true);

        labelText.text = ability.name;
        descriptionText.text = ability.description;

        PlayerMovement.Freeze();
    }

    public static void Disable()
    {
        if (!tutorial.activeSelf)
            return;

        PlayerMovement.UnFreeze();
        anim.SetTrigger("Exit");
    }

    private static void ChangeContinueText(string newName)
    {
        continueText.text = "Press ENTER to Continue";
    }

    public static bool IsActive()
    {
        return tutorial.activeSelf;
    }
}
