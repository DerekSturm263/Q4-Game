using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Follow : MonoBehaviour
{
    private SpriteRenderer _rndr;

    [SerializeField] private Transform _target;

    [SerializeField] private Vector2 _frequency;
    [SerializeField] private Vector2 _amplitude;
    [SerializeField] private float _lerpSpeed;

    [SerializeField] private Vector3 _offset;

    private void Awake()
    {
        _rndr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float x = Mathf.PerlinNoise1D(Time.time * _frequency.x) * _amplitude.x;
        float y = Mathf.PerlinNoise1D(Time.time * _frequency.y) * _amplitude.y;

        Vector3 targetPos = _target.transform.position + new Vector3(x, y) + _offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * _lerpSpeed);

        _rndr.flipX = transform.position.x > targetPos.x;
    }
}
