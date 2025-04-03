using Types.Miscellaneous;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EntityMovement : MonoBehaviour
{
    protected Rigidbody2D _rb;
    public Rigidbody2D Rigibody => _rb;

    protected Animator _anim;
    public Animator Animator => _anim;

    protected SpriteRenderer _rndr;
    public SpriteRenderer Renderer => _rndr;

    protected Mood _mood;
    public Mood Mood => _mood;

    [SerializeField] protected Range<float> _speed;
    public float WalkSpeed => _speed.Min;
    public float RunSpeed => _speed.Max;
    public float CurrentSpeed => (_isRunning ? _speed.Max : _speed.Min);

    protected bool _isMoving;
    protected bool _isRunning;

    protected Vector2 _lookDirection;
    public Vector2 LookDirection => _lookDirection;
    public void SetLookDirection(Vector2 lookDirection) => _lookDirection = lookDirection;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _rndr = GetComponentInChildren<SpriteRenderer>();
        _mood = GetComponent<Mood>();
    }

    protected virtual void Update()
    {
        _anim.SetFloat("XVelocity", _lookDirection.x * (_isMoving ? 1 : 0.1f));
        _anim.SetFloat("YVelocity", _lookDirection.y * (_isMoving ? 1 : 0.1f));
        _anim.SetFloat("Speed", _isRunning && _isMoving ? 2 : 1);
    }
}
