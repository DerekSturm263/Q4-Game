using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Endgame : MonoBehaviour
{
    void OnTriggerEnter2D()
    {
        SceneManager.LoadScene(2);
    }
}
