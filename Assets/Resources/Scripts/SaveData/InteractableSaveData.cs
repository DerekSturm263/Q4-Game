[System.Serializable]
public class InteractableSaveData
{
    public bool canUse;

    public InteractableSaveData(Interactable interactable)
    {
        this.canUse = interactable.canUse;
    }
}
