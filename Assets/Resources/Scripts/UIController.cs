using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject timeDisplay;
    public GameObject berryPrefab;

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
            TakeFood(2); // Food is taken away every day at dusk
        } else if ((int)rotCalc == -30 && timeTitle != "day")
        {
            timeTitle = "day";
            GiveFood(2); //Simulating the player finding food
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
            Instantiate(berryPrefab, new Vector3(-2.5f, 10, 0), Quaternion.identity);
        }

        Debug.Log(numFood);
    }

    public void TakeFood(int numFoodToTake)
    {
        GameObject[] berries;
        berries = GameObject.FindGameObjectsWithTag("UIBerry");

        Debug.Log("Berries Found: " + berries.Length);

        for (int i = 0; i < numFoodToTake; i++)
        {
            if (numFood == 0)
                break;
            berries[berries.Length - 1 - i].GetComponent<Rigidbody2D>().gravityScale = -1;
            numFood--;
            Debug.Log(numFood);
        }
    }

   
}
