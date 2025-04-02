using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable<PlayerMovement>
{
    [SerializeField] private string _interactType;
    [SerializeField] private UnityEvent<PlayerMovement> _onInteract;

    [SerializeField] private Types.Wrappers.Nullable<int> _interactCount;
    private int _interactsLeft;

    public string GetInteractType() => _interactType;

    private void Awake()
    {
        _interactsLeft = _interactCount.Value;
    }

    public void Interact(PlayerMovement player)
    {
        if (_interactCount.HasValue && _interactsLeft <= 0)
            return;

        _onInteract.Invoke(player);
        --_interactsLeft;
    }
}
