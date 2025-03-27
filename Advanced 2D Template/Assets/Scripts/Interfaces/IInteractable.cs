public interface IInteractable<TPlayerType>
{
    public string GetInteractType();

    public void Interact(TPlayerType player);
}
