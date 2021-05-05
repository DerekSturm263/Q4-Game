using UnityEngine;

public class AbilityUnlock : Interactable
{
    [SerializeField] private byte ability;

    public override void Effect()
    {
        if (!canUse)
            return;

        Debug.Log("Button Pressed");
        PlayerMovement.UnlockAbility(ability);

        canUse = false;
    }
}
