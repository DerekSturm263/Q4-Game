using Extensions;
using Types.Casting;
using Types.Miscellaneous;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _anim;

    [SerializeField] private Caster _interactCast;
    [SerializeField] private float _castOffset;

    [SerializeField] private Range<float> _movementSpeed;

    private Vector2 _direction;
    public Vector2 LookDirection => _direction.SnapTo4Slices();

    private bool _isRunning;
    public float MovementSpeed => (_isRunning ? _movementSpeed.Max : _movementSpeed.Min);

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        _rb.linearVelocity = _direction * MovementSpeed;

        _anim.SetFloat("XVelocity", _rb.linearVelocityX);
        _anim.SetFloat("YVelocity", _rb.linearVelocityY);
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        _direction = ctx.ReadValue<Vector2>();
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() == 0)
            return;

        var hit = _interactCast.GetHitInfo(transform, LookDirection * _castOffset);

        if (hit.HasValue && hit.Value.transform.TryGetComponent(out IInteractable onInteract))
        {
            onInteract.Interact(this);
           _anim.SetTrigger(onInteract.GetInteractType());
        }
    }

    public void Run(InputAction.CallbackContext ctx)
    {
        _isRunning = ctx.ReadValue<float>() == 1;
    }

    private void OnDrawGizmos()
    {
        _interactCast.Draw(transform, _direction * _castOffset);
    }
}
