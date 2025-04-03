using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    private LinkedListNode<State<T>> _defaultState;

    private LinkedListNode<State<T>> _previousState;
    private LinkedListNode<State<T>> _currentState;
    private LinkedListNode<State<T>> _nextState;

    private float _totalTime;

    public StateMachine(LinkedList<State<T>> states)
    {
        _currentState = states.First;
        _defaultState = states.First;
    }

    public void Resolve(T t, float deltaTime)
    {
        if (_nextState is not null)
        {
            _previousState = _currentState;
            _currentState = _nextState;
            
            _previousState.Value.Exit(t, this);
            _nextState.Value.Enter(t, this);

            _totalTime = 0f;

            _nextState = null;
        }
        else
        {
            _currentState.Value.Update(t, this, deltaTime, _totalTime);
            _totalTime += deltaTime;
        }
    }

    public void Transition()
    {
        _nextState = _currentState.Next;
    }
    
    public void Reset()
    {
        _nextState = _defaultState;
    }
}

public class State<T>
{
    private System.Action<T, StateMachine<T>> _onEnter;
    public void Enter(T t, StateMachine<T> machine) => _onEnter.Invoke(t, machine);

    private System.Action<T, StateMachine<T>, float, float> _onUpdate;
    public void Update(T t, StateMachine<T> machine, float deltaTime, float totalTime) => _onUpdate.Invoke(t, machine, deltaTime, totalTime);
    
    private System.Action<T, StateMachine<T>> _onExit;
    public void Exit(T t, StateMachine<T> machine) => _onExit.Invoke(t, machine);

    public static readonly State<EnemyMovement> Patrol = new()
    {
        _onEnter = (movement, machine) =>
        {

        },
        _onUpdate = (movement, machine, dt, time) =>
        {
            var hit = movement.PlayerCast.GetHitInfo(movement.transform);

            if (hit.HasValue)
            {
                var cast = Physics2D.Linecast(movement.transform.position, hit.Value.point, movement.AggroLayer);

                if (cast.transform == hit.Value.transform)
                {
                    movement.SetAggroedObject(hit.Value.transform);
                    machine.Transition();
                }
            }

            if (Vector2.Distance(movement.transform.position, movement.NextPos) <= movement.DistanceToNextSpot)
            {
                movement.DecreaseTimeRemaining();
                movement.Rigibody.linearVelocity = Vector2.zero;

                if (movement.TimeRemaining <= 0)
                {
                    movement.SetNextPos();
                }
            }
            else
            {
                movement.Rigibody.linearVelocity = (movement.NextPos - (Vector2)movement.transform.position).normalized * movement.WalkSpeed;
                movement.ResetTimeRemaining();
            }

            if (movement.Rigibody.linearVelocity != Vector2.zero)
                movement.SetLookDirection(movement.Rigibody.linearVelocity.normalized);
        },
        _onExit = (movement, machine) =>
        {

        }
    };

    public static readonly State<EnemyMovement> Chase = new()
    {
        _onEnter = (movement, machine) =>
        {
            movement.Mood.SetType(Mood.Type.Exclamation);
        },
        _onUpdate = (movement, machine, dt, time) =>
        {
            if (time < 0.5f)
            {
                movement.Rigibody.linearVelocity = Vector2.zero;
                return;
            }

            movement.Rigibody.linearVelocity = (movement.AggroedObject.position - movement.transform.position).normalized * movement.RunSpeed;

            var cast = Physics2D.Linecast(movement.transform.position, movement.AggroedObject.transform.position - new Vector3(0, 0.1f), movement.AggroLayer);
            if (cast.transform != movement.AggroedObject)
            {
                machine.Transition();
                movement.SetAggroedObject(null);
            }

            if (movement.Rigibody.linearVelocity != Vector2.zero)
                movement.SetLookDirection(movement.Rigibody.linearVelocity.normalized);
        },
        _onExit = (movement, machine) =>
        {
            movement.Mood.SetType(Mood.Type.None);
        }
    };

    public static readonly State<EnemyMovement> Confused = new()
    {
        _onEnter = (movement, machine) =>
        {
            movement.Mood.SetType(Mood.Type.Question);
        },
        _onUpdate = (movement, machine, dt, time) =>
        {
            movement.Rigibody.linearVelocity = Vector2.zero;

            if (time > 2)
                machine.Reset();
        },
        _onExit = (movement, machine) =>
        {
            movement.Mood.SetType(Mood.Type.None);
        }
    };

    public static readonly State<EnemyMovement> Look = new()
    {
        _onEnter = (movement, machine) =>
        {

        },
        _onUpdate = (movement, machine, dt, time) =>
        {
            movement.Rigibody.linearVelocity = Vector2.zero;
            movement.SetLookDirection((movement.AggroedObject.position - movement.transform.position).normalized);

            var hit = movement.PlayerCast.GetHitInfo(movement.transform);
            if (!hit.HasValue)
            {
                machine.Reset();
                movement.SetAggroedObject(null);
            }
        },
        _onExit = (movement, machine) =>
        {

        }
    };
}
