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

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        var hit = _playerCast.GetHitInfo(transform);

        if (hit.HasValue)
        {
            _rb.linearVelocity = (hit.Value.transform.position - transform.position).normalized * _speed.Max;
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }

        _anim.SetFloat("XVelocity", _rb.linearVelocityX);
        _anim.SetFloat("YVelocity", _rb.linearVelocityY);
    }

    private void OnDrawGizmos()
    {
        _playerCast.Draw(transform);
    }

    public void LoadSceneParameters()
    {
        SceneController.Instance.SetSceneParameter("Stats", _stats);
    }
}
