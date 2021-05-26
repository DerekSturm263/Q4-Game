using UnityEngine;
using UnityEngine.EventSystems;

public class TitleButtons : MonoBehaviour
{
    private EventSystem events;
    public Animator anim;

    public GameObject startButton;
    public GameObject gameButtons;

    public GameObject loadGameButton;

    public GameObject settings;

    private void Awake()
    {
        events = EventSystem.current;

        if (!MusicPlayer.Exists())
        {
            MusicPlayer.Initialize();
            SoundPlayer.Initialize();

            MusicPlayer.Play(0, "Lost in the Woods (Main Theme)");
            MusicPlayer.Play(1, "Lost in the Ocean (Underwater Theme)");
            MusicPlayer.SetVolume(0, 1f);
            MusicPlayer.SetVolume(1, 0f);

            SoundPlayer.SetVolume(0.5f);

            GameController.musicVolume = MusicPlayer.volume[0];
            GameController.musicVolume2 = MusicPlayer.volume[0] * 0.6f;
        }

        loadGameButton.SetActive(SaveDataController.HasSave());
    }

    public void ClickStart()
    {
        SoundPlayer.Play("ui_select");
        startButton.GetComponent<Animator>().enabled = true;
        gameButtons.SetActive(true);
    }

    public void Credits()
    {
        SoundPlayer.Play("ui_select");
        anim.SetTrigger("Exit");
    }

    public void NewGame()
    {
        SoundPlayer.Play("ui_select");
        anim.SetTrigger("Exit2");
        GameController.newGame = true;
    }

    public void LoadGame()
    {
        SoundPlayer.Play("ui_select");
        anim.SetTrigger("Exit2");
        GameController.newGame = false;
    }

    public void OpenSettings()
    {
        SoundPlayer.Play("ui_select");
        settings.SetActive(true);
    }

    public void Quit()
    {
        SoundPlayer.Play("ui_select");
        Application.Quit();
    }

    public void Back()
    {
        SoundPlayer.Play("ui_select");

        settings.GetComponent<Animator>().SetTrigger("Exit");
        events.SetSelectedGameObject(GetComponentsInChildren<UnityEngine.UI.Button>()[1].gameObject);
    }
}
