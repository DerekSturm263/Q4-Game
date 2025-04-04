using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable<PlayerMovement>
{
    [SerializeField] private string _interactType;
    [SerializeField] private Types.Collections.Directional<UnityEvent<PlayerMovement>> _onInteract;

    [SerializeField] private Types.Wrappers.Nullable<int> _interactCount;
    private int _interactsLeft;

    public string GetInteractType() => _interactType;

    private void Awake()
    {
        _interactsLeft = _interactCount.Value;
    }

    public void Interact(Transform user, PlayerMovement player)
    {
        if (_interactCount.HasValue && _interactsLeft <= 0)
            return;

        Vector2 difference = user.transform.position - transform.position;

        _onInteract[difference].Invoke(player);
        --_interactsLeft;
    }

    public bool CanInteract(Transform user)
    {
        Vector2 difference = user.transform.position - transform.position;

        return (!_interactCount.HasValue || _interactsLeft > 0) && _onInteract[difference].GetPersistentEventCount() > 0;
    }
}
