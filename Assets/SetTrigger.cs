using UnityEngine;

public class SetTrigger : StateMachineBehaviour
{
    public string parameter;
    public bool boolean;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(parameter, boolean);
    }
}
