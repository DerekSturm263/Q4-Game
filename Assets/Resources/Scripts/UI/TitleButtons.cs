using UnityEngine;

public class TitleButtons : MonoBehaviour
{
    public Animator anim;

    public GameObject startButton;
    public GameObject gameButtons;

    public GameObject loadGameButton;

    private void Awake()
    {
        if (!MusicPlayer.Exists())
        {
            MusicPlayer.Initialize();
            SoundPlayer.Initialize();

            MusicPlayer.Play(0, "Lost in the Woods (Main Theme)");
            MusicPlayer.Play(1, "Lost in the Ocean (Underwater Theme)");
            MusicPlayer.SetVolume(0, 0.5f);
            MusicPlayer.SetVolume(1, 0f);

            SoundPlayer.SetVolume(0.5f);

            GameController.musicVolume = MusicPlayer.volume[0];
            GameController.musicVolume2 = MusicPlayer.volume[0] * 0.3f;
        }

        loadGameButton.SetActive(SaveDataController.HasSave());
    }

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
