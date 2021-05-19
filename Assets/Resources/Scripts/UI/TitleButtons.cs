using UnityEngine;

public class TitleButtons : MonoBehaviour
{
    public Animator anim;

    public GameObject startButton;
    public GameObject gameButtons;

    public void ClickStart()
    {
        startButton.GetComponent<Animator>().enabled = true;
        gameButtons.SetActive(true);
    }

    public void Credits()
    {
        anim.SetTrigger("Exit");
    }

    public void NewGame()
    {
        anim.SetTrigger("Exit2");
        GameController.newGame = true;
    }

    public void LoadGame()
    {
        anim.SetTrigger("Exit2");
        GameController.newGame = false;
    }

    public void OpenSettings()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
