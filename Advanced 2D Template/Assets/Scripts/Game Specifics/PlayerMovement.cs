using SingletonBehaviours;
using Types.Casting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : EntityMovement
{
    [SerializeField] private Caster2D _interactCast;
    [SerializeField] private float _castOffset;

    private Vector2 _direction;

    public Vector3 InteractOffset => _lookDirection * _castOffset;

    protected override void Update()
    {
        _rb.linearVelocity = _direction * Speed;

        base.Update();
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        Vector2 value = ctx.ReadValue<Vector2>();

        _isMoving = value != Vector2.zero;
        _direction = value;

        if (value != Vector2.zero)
            _lookDirection = value;
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() == 0)
            return;

        var hit = _interactCast.GetHitInfo(transform, InteractOffset);

        if (hit.HasValue && hit.Value.transform.TryGetComponent(out IInteractable<PlayerMovement> onInteract))
        {
            onInteract.Interact(this);
           _anim.SetTrigger(onInteract.GetInteractType());
        }
    }

    public void Run(InputAction.CallbackContext ctx)
    {
        _isRunning = ctx.ReadValue<float>() == 1;
    }

    public void UseAbility(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() == 0)
            return;


        if (SaveDataController.Instance && SaveDataController.Instance.CurrentData.Mask)
        {
            SaveDataController.Instance.CurrentData.Mask.InvokeAction(this);
        }
    }

    private void OnDrawGizmos()
    {
        _interactCast.Draw(transform, InteractOffset);
    }
}
