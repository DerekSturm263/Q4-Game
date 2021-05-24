using UnityEngine;

public class FreezeTime : StateMachineBehaviour
{
    public float newTime;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Time.timeScale = newTime;
    }
}
