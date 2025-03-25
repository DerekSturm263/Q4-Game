using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactType;
    [SerializeField] private UnityEvent<PlayerMovement> _onInteract;

    public string GetInteractType() => _interactType;

    public void Interact(PlayerMovement player)
    {
        _onInteract.Invoke(player);
    }
}
