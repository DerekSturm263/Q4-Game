using UnityEngine;

public class InteractableSaveData : MonoBehaviour
{
    public bool canUse;

    public InteractableSaveData(Interactable interactable)
    {
        canUse = interactable.canUse;
    }
}
