using UnityEngine;

public class PlayerRespawn : StateMachineBehaviour
{
    public static PlayerMovement player;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.Respawn();
    }
}
