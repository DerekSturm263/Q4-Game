using Types.Wrappers;
using UnityEngine;
using UnityEngine.Events;

public class ScriptableCollisionEvents : MonoBehaviour
{
    [SerializeField] private Nullable<string> _tag;

    [SerializeField] private UnityEvent<GameObject> _onEnter;
    [SerializeField] private UnityEvent<GameObject> _onStay;
    [SerializeField] private UnityEvent<GameObject> _onExit;

    private void OnCollisionEnter(Collision collision)
    {
        if (!_tag.HasValue || collision.gameObject.CompareTag(_tag.Value))
            _onEnter.Invoke(collision.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_tag.HasValue || collision.gameObject.CompareTag(_tag.Value))
            _onEnter.Invoke(collision.gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_tag.HasValue || other.gameObject.CompareTag(_tag.Value))
            _onEnter.Invoke(other.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_tag.HasValue || collision.gameObject.CompareTag(_tag.Value))
            _onEnter.Invoke(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!_tag.HasValue || collision.gameObject.CompareTag(_tag.Value))
            _onStay.Invoke(collision.gameObject);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!_tag.HasValue || collision.gameObject.CompareTag(_tag.Value))
            _onStay.Invoke(collision.gameObject);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!_tag.HasValue || other.gameObject.CompareTag(_tag.Value))
            _onStay.Invoke(other.gameObject);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_tag.HasValue || collision.gameObject.CompareTag(_tag.Value))
            _onStay.Invoke(collision.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!_tag.HasValue || collision.gameObject.CompareTag(_tag.Value))
            _onExit.Invoke(collision.gameObject);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!_tag.HasValue || collision.gameObject.CompareTag(_tag.Value))
            _onExit.Invoke(collision.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!_tag.HasValue || other.gameObject.CompareTag(_tag.Value))
            _onExit.Invoke(other.gameObject);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!_tag.HasValue || collision.gameObject.CompareTag(_tag.Value))
            _onExit.Invoke(collision.gameObject);
    }
}
