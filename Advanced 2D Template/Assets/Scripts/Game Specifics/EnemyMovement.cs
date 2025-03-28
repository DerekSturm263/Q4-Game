using SingletonBehaviours;
using System.Collections.Generic;
using Types.Casting;
using Types.Miscellaneous;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _anim;

    [SerializeField] private List<EntityStats> _stats;

    [SerializeField] private Range<float> _speed;
    [SerializeField] private Caster2D _playerCast;
    [SerializeField] private LayerMask _aggroLayer;

    private Transform _aggroedObject;

    [SerializeField] private Range<Vector2> _patrolDistance;
    [SerializeField] private float _distanceToNextSpot;

    private Vector2 _originalPos;
    private Vector2 _nextPos;

    [SerializeField] private Range<float> _waitTime;

    private float _timeRemaining;

    private bool _isMoving;
    private Vector2 _lookDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        _originalPos = transform.position;
        SetNextPos();
    }

    private void Update()
    {
        if (_aggroedObject)
        {
            var cast = Physics2D.Linecast(transform.position,  _aggroedObject.transform.position, _aggroLayer);

            if (cast.transform == _aggroedObject)
            {
                Chase();
            }
            else
            {
                _aggroedObject = null;
            }
        }
        else
        {
            var hit = _playerCast.GetHitInfo(transform);

            if (hit.HasValue)
            {
                var cast = Physics2D.Linecast(transform.position, hit.Value.point, _aggroLayer);

                if (cast.transform == hit.Value.transform)
                {
                    _aggroedObject = hit.Value.transform;
                    Chase();
                }
                else
                {
                    Patrol();
                }
            }
            else
            {
                Patrol();
            }
        }

        if (_rb.linearVelocity != Vector2.zero)
            _lookDirection = _rb.linearVelocity.normalized;

        _anim.SetFloat("XVelocity", _lookDirection.x * (_isMoving ? 1 : 0.1f));
        _anim.SetFloat("YVelocity", _lookDirection.y * (_isMoving ? 1 : 0.1f));
        _anim.SetFloat("Speed", 1);
    }

    private void Chase()
    {
        _rb.linearVelocity = (_aggroedObject.position - transform.position).normalized * _speed.Max;
        _isMoving = true;
    }

    private void Patrol()
    {
        if (Vector2.Distance(transform.position, _nextPos) <= _distanceToNextSpot)
        {
            _timeRemaining -= Time.deltaTime;
            _rb.linearVelocity = Vector2.zero;
            _isMoving = false;

            if (_timeRemaining <= 0)
            {
                SetNextPos();
            }
        }
        else
        {
            _rb.linearVelocity = (_nextPos - (Vector2)transform.position).normalized * _speed.Min;
            _timeRemaining = Random.Range(_waitTime.Min, _waitTime.Max);

            _isMoving = true;
        }
    }

    private void SetNextPos()
    {
        Vector2 offset = new(Random.Range(_patrolDistance.Min.x, _patrolDistance.Max.x), Random.Range(_patrolDistance.Min.y, _patrolDistance.Max.y));
        _nextPos = _originalPos + offset;
    }

    private void OnDrawGizmos()
    {
        _playerCast.Draw(transform);

        if (_aggroedObject)
            Gizmos.DrawLine(transform.position, _aggroedObject.transform.position);
    }

    public void LoadSceneParameters()
    {
        SceneController.Instance.SetSceneParameter("Stats", _stats);
    }
}
