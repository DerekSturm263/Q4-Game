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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Display("Nighttime Dangers", "As night approaches, the forest becomes a dangerous place." +
                "Monsters will begin spawning in unsual places, and you must be prepared for anything;" +
                "however, each night that you succesfully survive grants you extra berries.");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Display("Feeding The Tribe", "Each night, you must have enough berries to feed the tribe, otherwise it's game over." +
                "The tribe requires 10 berries every night to survive." +
                "You can find berries around the world by solving puzzles and exploring.");
        }
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
