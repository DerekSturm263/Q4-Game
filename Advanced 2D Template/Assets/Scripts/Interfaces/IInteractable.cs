using UnityEngine;

public interface IInteractable<TPlayerType>
{
    public string GetInteractType();

    public void Interact(Transform user, TPlayerType player);
    public bool CanInteract(Transform user);
}
