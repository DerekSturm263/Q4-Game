using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
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

    public static float numFood;
    public static float timePassedSinceGameBegun = 0f;

    public float takeAwayBerries; //How many berries are taken away each dusk

    public static bool sendDuskMessage = true;
    public static bool sendNightMessage = true;

    public GameObject GameOverImage;

    private void Awake()
    {
        events = EventSystem.current;
        ui = this;

        cycleSeconds = cycleLength * 60;
        foodNumDisplay.text = "" + numFood;
        time = cycleSeconds * 0.075f;
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
            TakeFood((int)takeAwayBerries); // Food is taken away every day at night
            SoundPlayer.Play("food_time");

            if (sendNightMessage && numDays == 1)
            {
                LoadTutorial.Display("Nighttime Dangers", "As night approaches, the forest becomes a dangerous place. " +
                    "Monsters will begin spawning in unusual places, and you must be prepared for anything. " +
                    "However, it is easier to spot berries and other items at night.");
                sendNightMessage = false;
            }
        } else if ((int)rotCalc == -150 && timeTitle != "dusk")
        {
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
            timeTitle = "dawn";
        }

        //Debug.Log((int)rotCalc + ": " + timeTitle);
        //Debug.Log(timeTitle);
        //Debug.Log(timeDisplayRoation.eulerAngles);

        //Pause Menu
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
    }

    public void ToTitle()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }

    public void Save()
    {
        GameController.TrySaveGame();
        Resume();
    }

    public void OpenSettings()
    {

    }

    public void Resume()
    {
        pauseMenu.GetComponent<Animator>().SetTrigger("Exit");
        isPaused = false;
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        isPaused = true;

        events.SetSelectedGameObject(pauseMenu.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
    }

    public void GameOver()
    {
        GameOverImage.SetActive(true);
    }
}
