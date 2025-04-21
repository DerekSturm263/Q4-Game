using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    private Camera _cam;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _sizeMultiplier;
    [SerializeField] private float _sizeSpeed;

    private Vector3 _targetPos;
    private float _targetSize;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * _moveSpeed);
        _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _targetSize, Time.deltaTime * _sizeSpeed);
    }

    public void SetTarget(Collider2D target)
    {
        _targetPos = target.bounds.center;
        _targetPos.z = -10;

        _targetSize= target.bounds.size.magnitude * _sizeMultiplier;
    }
}
