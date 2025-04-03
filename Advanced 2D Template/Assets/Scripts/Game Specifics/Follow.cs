using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Follow : MonoBehaviour
{
    private SpriteRenderer _rndr;

    [SerializeField] private Transform _player;

    [SerializeField] private Transform _target;
    public void SetTarget(Transform target) => _target = target;

    [SerializeField] private Vector2 _frequency;
    [SerializeField] private Vector2 _amplitude;
    [SerializeField] private float _lerpSpeed;
    [SerializeField] private float _leadDistance;
    public float SetLeadDistance(float leadDistance) => _leadDistance = leadDistance;

    [SerializeField] private Vector3 _offset;

    public bool IsLeading => _target != _player;

    private void Awake()
    {
        _rndr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float x = Mathf.PerlinNoise1D(Time.time * _frequency.x) * _amplitude.x;
        float y = Mathf.PerlinNoise1D(Time.time * _frequency.y) * _amplitude.y;

        Vector3 targetPos;
        if (_target == _player)
        {
            targetPos = _target.transform.position + new Vector3(x, y) + _offset;
        }
        else
        {
            targetPos = _player.transform.position + ((_target.transform.position - _player.transform.position).normalized * _leadDistance) + new Vector3(x, y) + _offset;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * _lerpSpeed);
        _rndr.flipX = transform.position.x > targetPos.x;
    }

    public void Lead(Transform target)
    {
        SetTarget(target);

        if (TryGetComponent(out LightFlicker flicker))
        {
            flicker.SetExcited();
        }
    }

    public void EndLead()
    {
        SetTarget(_player);

        if (TryGetComponent(out LightFlicker flicker))
        {
            flicker.SetDefault();
        }
    }
}
