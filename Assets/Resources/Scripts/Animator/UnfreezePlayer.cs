using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnfreezePlayer : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!LoadTutorial.tutorial.activeSelf)
        {
            PlayerMovement.lockMovement = false;
        }
    }
}
