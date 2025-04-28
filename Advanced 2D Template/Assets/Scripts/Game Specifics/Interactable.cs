using SingletonBehaviours;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable<PlayerMovement>, ISavable
{
    private Animator _anim;

    [SerializeField] private string _interactType;
    [SerializeField] private Types.Collections.Directional<UnityEvent<PlayerMovement>> _onInteract;

    [SerializeField] private Types.Wrappers.Nullable<int> _interactCount;
    private int _interactsLeft;

    public string GetInteractType() => _interactType;

    private void Awake()
    {
        _anim = GetComponent<Animator>();

        if (SaveDataController.Instance.CurrentData.InteractStates.TryGetValue(name, out var interactCount))
        {
            _interactsLeft = interactCount.Item2;
            
            if (_anim)
                _anim.Play(interactCount.Item1, 0);
        }
        else
        {
            _interactsLeft = _interactCount.Value;
        }
    }

    public void Interact(Transform user, PlayerMovement player)
    {
        if (_interactCount.HasValue && _interactsLeft <= 0)
            return;

        Vector2 difference = user.transform.position - transform.position;

        _onInteract[difference].Invoke(player);
        --_interactsLeft;

        if (!SaveDataController.Instance.CurrentData.InteractStates.ContainsKey(name))
            SaveDataController.Instance.CurrentData.InteractStates.Add(name, default);
    }

    public bool CanInteract(Transform user)
    {
        Vector2 difference = user.transform.position - transform.position;

        return (!_interactCount.HasValue || _interactsLeft > 0) && _onInteract[difference].GetPersistentEventCount() > 0;
    }

    public void Save()
    {
        SaveDataController.Instance.CurrentData.InteractStates[name] = new(_anim ? _anim.GetCurrentAnimatorStateInfo(0).fullPathHash : 0, _interactsLeft);
    }
}
