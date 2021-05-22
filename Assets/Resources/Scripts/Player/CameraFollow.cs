using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followObj;
    private Vector3 offset = new Vector3(0f, 0f, -12.5f);
    public float speed = 5f;

    public static Vector3 defaultOffset = new Vector3(0f, 0f, -12.5f);
    public static Vector3 lookUpOffset = new Vector3(0f, 2.5f, -12.5f);

    private void Awake()
    {
        transform.position = followObj.position + offset;
    }

    private void FixedUpdate()
    {
        Vector3 camPos = new Vector3(Mathf.Clamp(followObj.position.x + offset.x, -478f, 563f), followObj.position.y + offset.y, followObj.position.z + offset.z);
        transform.position = Vector3.Lerp(transform.position, camPos, Time.deltaTime * speed);
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    public Vector3 GetOffset()
    {
        return offset;
    }
}
