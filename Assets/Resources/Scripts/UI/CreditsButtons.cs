using UnityEngine;

public class CreditsButtons : MonoBehaviour
{
    public Animator anim;

    public void LoadTitle()
    {
        anim.SetTrigger("Exit");
    }
}
