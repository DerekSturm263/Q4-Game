using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
    private PlayerMovement player;

    private static UIController ui;
    private EventSystem events;

    public GameObject timeDisplay;
    public GameObject berryPrefab;
    public TextMeshProUGUI foodNumDisplay;

    public static float time;
    public static string timeTitle;  //Ex: "dawn", "day" ...
    public float cycleLength;  //Minutes for a full day/night cycle
    public static int numDays = 1;
    private float cycleSeconds;
    private Quaternion timeDisplayRoation;

    public static bool isPaused = false;
    public GameObject pauseMenu;
    public static GameObject pauseMenuStatic;

    public static float numFood;
    public static float timePassedSinceGameBegun = 0f;

    public int takeAwayBerries; //How many berries are taken away each dusk

    public static bool sendDuskMessage = true;
    public static bool sendNightMessage = true;
    public static bool startSunLate = true;

    public GameObject GameOverImage;
    public GameObject settings;

    public bool useCheats;

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        events = EventSystem.current;
        ui = this;
        isPaused = false;

        pauseMenuStatic = pauseMenu;

        cycleSeconds = cycleLength * 60;
        foodNumDisplay.text = "" + numFood;

        if (startSunLate)
        {
            time = cycleSeconds * 0.075f;
            startSunLate = false;
        }
    }
    
    void Update()
    {
        time += Time.deltaTime;
        timePassedSinceGameBegun += Time.deltaTime;

        float rotCalc = -((time / cycleSeconds) * 360);
        if (rotCalc <= -360)
        {
            time = 0;
            numDays++;

            GameController.TryAutoSaveGame();
        }
        Vector3 rotationVector = new Vector3(0, 0, rotCalc);
        timeDisplayRoation.eulerAngles = rotationVector;

        timeDisplay.transform.rotation = timeDisplayRoation;

        if ((int)rotCalc == -180 && timeTitle != "night")
        {
            timeTitle = "night";
            GameController.SpawnEnemies();
            TakeFood(takeAwayBerries); // Food is taken away every day at night

            if (sendNightMessage && numDays == 1 && numFood >= 10)
            {
                LoadTutorial.Display("Nighttime Dangers", "As night approaches, the forest becomes a dangerous place. " +
                    "Red monsters will begin spawning in unusual places, and you must be prepared for anything. " +
                    "However, it is easier to spot berries and other items at night.");
                sendNightMessage = false;
            }
        } else if ((int)rotCalc == -150 && timeTitle != "dusk")
        {
            SoundPlayer.Play("food_time");
            timeTitle = "dusk";
            if (sendDuskMessage && numDays == 1)
            {
                LoadTutorial.Display("Feeding The Tribe", "Each night, you must have enough berries to feed the tribe, otherwise it's game over. " +
                "The tribe requires 10 berries every night to survive. " +
                "You can find berries around the world by solving puzzles and exploring.");
                sendDuskMessage = false;
            }
        } else if ((int)rotCalc == -30 && timeTitle != "day")
        {
            timeTitle = "day";
            //GiveFood(5); //Simulating the player finding food
        } else if ((int)rotCalc == 0 && timeTitle != "dawn")
        {
            GameController.DespawnEnemies();
            timeTitle = "dawn";
        }

        //Debug.Log((int)rotCalc + ": " + timeTitle);
        //Debug.Log(timeTitle);
        //Debug.Log(timeDisplayRoation.eulerAngles);

        //Pause Menu

        if (useCheats && Input.GetKeyDown(KeyCode.P))
        {
            time += (cycleSeconds * 0.05f);
        }
    }

    public void GiveFood(int numFoodToGive)
    {
        for (int i = 0; i < numFoodToGive; i++)
        {
            numFood++;
            // need to change this out later to be based on screen size
            //Instantiate(berryPrefab, new Vector3(-2.5f, 10, 0), Quaternion.identity);
        }

        foodNumDisplay.text = "" + numFood; 
        //Debug.Log(numFood);
    }

    // DO NOT Call Take food too quickly, it will break if it is called before
    // the other berries are gone
    public void TakeFood(int numFoodToTake)
    {
        //GameObject[] berries;
        //berries = GameObject.FindGameObjectsWithTag("UIBerry");

        //Debug.Log("Berries Found: " + berries.Length);

        // Check if has enough food (use this area for player punishment)
        if (numFoodToTake > numFood)
        {
            GameOver();
            //Debug.Log("NOT ENOUGH FOOD");
        }

        for (int i = 0; i < numFoodToTake; i++)
        {
            if (numFood == 0)
                break;
            //berries[berries.Length - 1 - i].GetComponent<Rigidbody2D>().gravityScale = -1;
            numFood--;

            foodNumDisplay.text = "" + numFood;
            //Debug.Log(numFood);
        }
    }

    public static void TryPause()
    {
        Debug.Log("Pause");

        if (isPaused)
        {
            ui.Resume();
        }
        else
        {
            ui.Pause();
        }
    }

    public void ToCredits()
    {
        Time.timeScale = 1f;
        SoundPlayer.Play("ui_select");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
    }

    public void ToTitle()
    {
        Time.timeScale = 1f;
        SoundPlayer.Play("ui_select");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }

    public void Save()
    {
        SoundPlayer.Play("ui_select");
        GameController.TrySaveGame();
        Resume();
    }

    public void LoadScene()
    {
        Time.timeScale = 1f;
        SoundPlayer.Play("ui_select");
        player.Respawn();
        GameController.TryLoadAutoSaveGame();
        GameOverImage.SetActive(false);
    }

    public void OpenSettings()
    {
        SoundPlayer.Play("ui_select");
        settings.SetActive(true);
        Time.timeScale = 1f;

        events.SetSelectedGameObject(settings.GetComponentInChildren<UnityEngine.UI.Toggle>().gameObject);
    }

    public void Back()
    {
        SoundPlayer.Play("ui_select");
        settings.GetComponent<Animator>().SetTrigger("Exit");

        events.SetSelectedGameObject(settings.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
    }

    public void Resume()
    {
        SoundPlayer.Play("ui_select");
        pauseMenu.GetComponent<Animator>().SetTrigger("Exit");
        settings.SetActive(false);

        isPaused = false;
    }

    void Pause()
    {
        SoundPlayer.Play("ui_select");
        pauseMenu.SetActive(true);
        isPaused = true;

        events.SetSelectedGameObject(pauseMenu.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
    }

    public void GameOver()
    {
        GameOverImage.SetActive(true);
    }

    public static bool IsActive()
    {
        return pauseMenuStatic.activeSelf;
    }
}
