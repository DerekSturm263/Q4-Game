using SingletonBehaviours;
using System.Collections.Generic;
using Types.Casting;
using Types.Miscellaneous;
using UnityEngine;

public class EnemyMovement : EntityMovement
{
    [SerializeField] private List<EntityStats> _stats;

    [SerializeField] private Caster2D _playerCast;
    [SerializeField] private LayerMask _aggroLayer;

    private Transform _aggroedObject;

    [SerializeField] private Range<Vector2> _patrolDistance;
    [SerializeField] private float _distanceToNextSpot;

    private Vector2 _originalPos;
    private Vector2 _nextPos;

    [SerializeField] private Range<float> _waitTime;

    private float _timeRemaining;

    protected override void Awake()
    {
        base.Awake();

        _originalPos = transform.position;
        SetNextPos();
    }

    protected override void Update()
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

        base.Update();
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
