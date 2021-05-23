using UnityEngine;

public class Endgame : MonoBehaviour
{
    public GameObject fadeOut;

    void OnTriggerEnter2D()
    {
        fadeOut.SetActive(true);
    }
}
