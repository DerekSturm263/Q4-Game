using Extensions;
using Types.Casting;
using Types.Miscellaneous;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _anim;

    [SerializeField] private Caster2D _interactCast;
    [SerializeField] private float _castOffset;

    [SerializeField] private Range<float> _movementSpeed;

    private Vector2 _direction;
    private Vector2 _lookDirection;

    private bool _isMoving;
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

        _anim.SetFloat("XVelocity", _lookDirection.x * (_isMoving ? 1 : 0.1f));
        _anim.SetFloat("YVelocity", _lookDirection.y * (_isMoving ? 1 : 0.1f));
        _anim.SetFloat("Speed", _isRunning ? 2 : 1);
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

        var hit = _interactCast.GetHitInfo(transform, _lookDirection * _castOffset);

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

    private void OnDrawGizmos()
    {
        _interactCast.Draw(transform, _direction * _castOffset);
    }
}
