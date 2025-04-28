using Types.Miscellaneous;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class InfiniteScroll : MonoBehaviour
{
    [SerializeField] private Sprite[] _variations;

    [SerializeField] private Bounds _bounds;

    [SerializeField] private Range<Vector2> _velocity;
    private Vector2 _currentVelocity;

    [SerializeField] private Range<float> _size;
    [SerializeField] private Range<Vector2> _offset;

    private Vector2 _originalPos;

    private void Awake()
    {
        _originalPos = transform.position;

        ResetSettings();
    }

    private void Update()
    {
        transform.position += (Vector3)_currentVelocity * Time.deltaTime;

        if (transform.position.x < _bounds.min.x)
        {
            ResetSettings();

            transform.position = new(_bounds.max.x, transform.position.y, transform.position.z);
        }
    }

    private void ResetSettings()
    {
        GetComponent<SpriteRenderer>().sprite = _variations[Random.Range(0, _variations.Length)];

        _currentVelocity.x = Random.Range(_velocity.Min.x, _velocity.Max.x);
        _currentVelocity.y = Random.Range(_velocity.Min.y, _velocity.Max.y);

        transform.position = _originalPos + new Vector2(Random.Range(_offset.Min.x, _offset.Max.x), Random.Range(_offset.Min.y, _offset.Max.y));

        float scale = Random.Range(_size.Min, _size.Max);
        transform.localScale = new(scale, scale);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(_bounds.center, _bounds.size);
    }
}
