using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : StateMachineBehaviour
{
    public string scene;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
}
