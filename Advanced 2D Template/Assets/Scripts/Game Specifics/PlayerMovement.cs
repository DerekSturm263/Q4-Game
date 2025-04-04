using SingletonBehaviours;
using Types.Casting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : EntityMovement
{
    [SerializeField] private Caster2D _interactCast;
    [SerializeField] private float _castOffset;

    [SerializeField] private Transform _playerVisual;

    [SerializeField] private LayerMask _ignoreGrounded;
    [SerializeField] private LayerMask _ignoreJump;

    private Follow _flyFollow;
    private bool _isLeading;

    private Vector2 _direction;

    public Vector3 InteractOffset => _lookDirection * _castOffset;

    private bool _canInteract;
    public bool SetCanInteract(bool canInteract) => _canInteract = canInteract;

    [SerializeField] private AnimationCurve _jumpCurve;
    [SerializeField] private float _jumpLength;
    [SerializeField] private float _jumpMultiplier;
    private float _jumpTime;

    protected override void Awake()
    {
        base.Awake();

        _flyFollow = FindFirstObjectByType<Follow>();
        _canInteract = true;
    }

    protected override void Update()
    {
        _rb.linearVelocity = _direction * CurrentSpeed;

        if (_jumpTime > 0)
        {
            float height = _jumpCurve.Evaluate(_jumpTime) * _jumpMultiplier;

            _playerVisual.localPosition = new(0, height);

            _jumpTime += Time.deltaTime;

            if (_jumpTime > _jumpLength)
                EndJump();
        }
        else
        {
            _playerVisual.localPosition = Vector3.zero;
        }

        var hit = _interactCast.GetHitInfo(transform, InteractOffset);

        if (_canInteract && hit.HasValue && hit.Value.transform.TryGetComponent(out Interactable interactable) && interactable.CanInteract(transform))
        {
            _mood.SetType(Mood.Type.Interact);

            if (!_flyFollow.IsLeading)
            {
                _flyFollow.SetLeadDistance(0.5f);
                _flyFollow.Lead(hit.Value.transform);

                _isLeading = true;
            }
        }
        else
        {
            _mood.SetType(Mood.Type.None);

            if (_isLeading)
            {
                _flyFollow.EndLead();
                _flyFollow.SetLeadDistance(4);

                _isLeading = false;
            }
        }

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

    public void Run(InputAction.CallbackContext ctx)
    {
        _isRunning = ctx.ReadValue<float>() == 1;
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() == 0 || _jumpTime > 0)
            return;

        _jumpTime = 0.01f;
        _rndr.sortingOrder = 3;
        _col.excludeLayers = _ignoreJump;
    }

    public void EndJump()
    {
        _jumpTime = 0;
        _rndr.sortingOrder = 2;
        _col.excludeLayers = _ignoreGrounded;
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() == 0)
            return;

        var hit = _interactCast.GetHitInfo(transform, InteractOffset);

        if (hit.HasValue && hit.Value.transform.TryGetComponent(out IInteractable<PlayerMovement> onInteract))
        {
            onInteract.Interact(transform, this);
            _anim.SetTrigger(onInteract.GetInteractType());
        }
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

    public void SetInteractionEnabled(bool isEnabled)
    {
        var player = FindFirstObjectByType<PlayerMovement>();
        
        player._canInteract = isEnabled;
        player.GetComponent<PlayerInput>().enabled = isEnabled;
    }

    private void OnDrawGizmos()
    {
        _interactCast.Draw(transform, InteractOffset);
    }
}
