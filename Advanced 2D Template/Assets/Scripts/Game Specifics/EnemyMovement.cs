using SingletonBehaviours;
using System.Collections.Generic;
using Types.Casting;
using Types.Miscellaneous;
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

    [SerializeField] private List<MovementBehavior> _behaviors;

    private Transform _aggroedObject;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_aggroedObject)
        {
            var cast = Physics2D.Linecast(transform.position,  _aggroedObject.transform.position, _aggroLayer);

            if (cast.transform == _aggroedObject)
            {
                _rb.linearVelocity = (_aggroedObject.position - transform.position).normalized * _speed.Max;
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
                    _rb.linearVelocity = (hit.Value.transform.position - transform.position).normalized * _speed.Max;
                    _aggroedObject = hit.Value.transform;
                }
                else
                {
                    _rb.linearVelocity = Vector2.zero;
                }
            }
            else
            {
                _rb.linearVelocity = Vector2.zero;
            }
        }

        _anim.SetFloat("XVelocity", _rb.linearVelocityX);
        _anim.SetFloat("YVelocity", _rb.linearVelocityY);
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
