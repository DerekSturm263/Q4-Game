using UnityEngine;

public class AbilityUnlock : Interactable
{
    [SerializeField] private byte ability;

    public override void Effect()
    {
        Debug.Log("Button Pressed");
        PlayerMovement.UnlockAbility(ability);
    }
}
