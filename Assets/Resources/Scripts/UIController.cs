using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject timeDisplay;

    public static float time;
    public static string timeTitle;
    public float cycleLength;
    private float cycleSeconds;
    private Quaternion timeDisplayRoation;

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

        if ((int)rotCalc == -180)
        {
            timeTitle = "night";
        } else if ((int)rotCalc == -150)
        {
            timeTitle = "dusk";
        } else if ((int)rotCalc == -30)
        {
            timeTitle = "day";
        } else if ((int)rotCalc == 0)
        {
            timeTitle = "dawn";
        }

        //Debug.Log((int)rotCalc + ": " + timeTitle);
        //Debug.Log(timeTitle);
        //Debug.Log(timeDisplayRoation.eulerAngles);
    }
}
