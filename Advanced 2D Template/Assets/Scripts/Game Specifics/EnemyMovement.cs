using System.Collections;
using Types.Casting;
using Types.Miscellaneous;
using UnityEngine;

public class EnemyMovement : EntityMovement
{
    public enum NPCType
    {
        Enemy, Ally
    }

    [SerializeField] private Caster2D _playerCast;
    public Caster2D PlayerCast => _playerCast;

    [SerializeField] private LayerMask _aggroLayer;
    public LayerMask AggroLayer => _aggroLayer;

    private Transform _aggroedObject;
    public Transform AggroedObject => _aggroedObject;
    public void SetAggroedObject(Transform aggroedObject) => _aggroedObject = aggroedObject;

    [SerializeField] private Range<Vector2> _patrolDistance;

    [SerializeField] private float _distanceToNextSpot;
    public float DistanceToNextSpot => _distanceToNextSpot;

    private Vector2 _originalPos;

    private Vector2 _nextPos;
    public Vector2 NextPos => _nextPos;

    [SerializeField] private Range<float> _waitTime;

    private float _timeRemaining;
    public float TimeRemaining => _timeRemaining;
    public void DecreaseTimeRemaining() => _timeRemaining -= Time.deltaTime;
    public void ResetTimeRemaining() => _timeRemaining = Random.Range(_waitTime.Min, _waitTime.Max);

    private StateMachine<EnemyMovement> _stateMachine;

    [SerializeField] private NPCType _type;

    protected override void Awake()
    {
        base.Awake();

        System.Collections.Generic.LinkedList<State<EnemyMovement>> states = new();
        states.AddLast(State<EnemyMovement>.Patrol);
        
        if (_type == NPCType.Enemy)
        {
            states.AddLast(State<EnemyMovement>.Chase);
            states.AddLast(State<EnemyMovement>.Confused);
        }
        else
        {
            states.AddLast(State<EnemyMovement>.Look);
        }

        _stateMachine = new(states);

        _originalPos = transform.position;
        SetNextPos();
    }

    protected override void Update()
    {
        _stateMachine.Resolve(this, Time.deltaTime);

        _isMoving = _rb.linearVelocity != Vector2.zero;

        base.Update();
    }

    public void SetNextPos()
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
}
