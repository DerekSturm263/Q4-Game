using UnityEngine;
using UnityEngine.EventSystems;

public class Selector : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;

    private void Update()
    {
        if (EventSystem.current && EventSystem.current.currentSelectedGameObject)
            transform.position = EventSystem.current.currentSelectedGameObject.transform.position + _offset;
    }
}
