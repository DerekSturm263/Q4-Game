using UnityEngine;

public class CreditsButtons : MonoBehaviour
{
    public Animator anim;

    public void LoadTitle()
    {
        SoundPlayer.Play("ui_select");
        anim.SetTrigger("Exit");
    }
}
