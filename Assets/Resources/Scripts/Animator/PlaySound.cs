using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : StateMachineBehaviour
{
    private PlayerMovement player;
    public AudioClip[] sound;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
        {
            player = animator.GetComponent<PlayerMovement>();
        }

        player.PlaySound(sound, true, 1f, 0.5f);
    }
}
