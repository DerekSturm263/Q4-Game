using Types.Miscellaneous;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public abstract class EntityMovement : MonoBehaviour
{
    protected Rigidbody2D _rb;
    public Rigidbody2D Rigibody => _rb;

    protected Animator _anim;
    public Animator Animator => _anim;

    [SerializeField] protected Range<float> _speed;
    public float Speed => (_isRunning ? _speed.Max : _speed.Min);

    protected bool _isMoving;
    protected bool _isRunning;

    protected Vector2 _lookDirection;
    public Vector2 LookDirection => _lookDirection;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        _anim.SetFloat("XVelocity", _lookDirection.x * (_isMoving ? 1 : 0.1f));
        _anim.SetFloat("YVelocity", _lookDirection.y * (_isMoving ? 1 : 0.1f));
        _anim.SetFloat("Speed", _isRunning && _isMoving ? 2 : 1);
    }
}
