using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject timeDisplay;

    public float time;
    public float cycleLength;
    private float cycleSeconds;
    private Quaternion timeDisplayRoation;

    void Start()
    {
        cycleSeconds = cycleLength * 60;
        timeDisplayRoation = Quaternion.identity;
    }
    
    void Update()
    {
        Vector3 rotationVector = new Vector3(0, 0, -((time / cycleSeconds) * 360));
        timeDisplayRoation.eulerAngles = rotationVector;

        timeDisplay.transform.rotation = timeDisplayRoation;
        //Debug.Log(timeDisplayRoation.eulerAngles);
    }
}
