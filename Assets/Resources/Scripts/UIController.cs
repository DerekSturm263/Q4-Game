using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject timeDisplay;
    public GameObject berryPrefab;
    public TextMeshProUGUI foodNumDisplay;

    public static float time;
    public static string timeTitle;
    public float cycleLength;
    private float cycleSeconds;
    private Quaternion timeDisplayRoation;

    public static float numFood;

    void Start()
    {
        cycleSeconds = cycleLength * 60;
        timeDisplayRoation = Quaternion.identity;

        foodNumDisplay.text = "X " + numFood;
    }
    
    void FixedUpdate()
    {
        time += Time.deltaTime;
        float rotCalc = -((time / cycleSeconds) * 360);
        if (rotCalc <= -360)
            time = 0;
        Vector3 rotationVector = new Vector3(0, 0, rotCalc);
        timeDisplayRoation.eulerAngles = rotationVector;

        timeDisplay.transform.rotation = timeDisplayRoation;

        if ((int)rotCalc == -180 && timeTitle != "night")
        {
            timeTitle = "night";
        } else if ((int)rotCalc == -150 && timeTitle != "dusk")
        {
            timeTitle = "dusk";
            //TakeFood(5); // Food is taken away every day at dusk
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
    }

    public void GiveFood(int numFoodToGive)
    {
        for (int i = 0; i < numFoodToGive; i++)
        {
            numFood++;
            // need to change this out later to be based on screen size
            Instantiate(berryPrefab, new Vector3(-2.5f, 10, 0), Quaternion.identity);
        }

        foodNumDisplay.text = "X " + numFood; 
        //Debug.Log(numFood);
    }

    // DO NOT Call Take food too quickly, it will break if it is called before
    // the other berries are gone
    public void TakeFood(int numFoodToTake)
    {
        GameObject[] berries;
        berries = GameObject.FindGameObjectsWithTag("UIBerry");

        Debug.Log("Berries Found: " + berries.Length);

        // Check if has enough food (use this area for player punishment)
        if (numFoodToTake > numFood)
        {
            Debug.Log("NOT ENOUGH FOOD");
        }

        for (int i = 0; i < numFoodToTake; i++)
        {
            if (numFood == 0)
                break;
            berries[berries.Length - 1 - i].GetComponent<Rigidbody2D>().gravityScale = -1;
            numFood--;

            foodNumDisplay.text = "X " + numFood;
            //Debug.Log(numFood);
        }
    }

   
}
