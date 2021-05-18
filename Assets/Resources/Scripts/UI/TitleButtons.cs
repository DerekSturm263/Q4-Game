using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleButtons : MonoBehaviour
{
    public GameObject startButton;
    public GameObject gameButtons;

    public void ClickStart()
    {
        startButton.GetComponent<Animator>().enabled = true;
        gameButtons.SetActive(true);
    }

    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
