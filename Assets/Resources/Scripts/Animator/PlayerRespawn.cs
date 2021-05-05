using UnityEngine;

public class PlayerRespawn : StateMachineBehaviour
{
    public static PlayerMovement player;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.Respawn();
        EntityAI.entities.ForEach((x) =>
        {
            EntityAI enemy = x.GetComponent<EntityAI>();

            if (enemy.isActive)
            {
                x.GetComponent<EntityAI>().Respawn();
            }
        });
    }
}
