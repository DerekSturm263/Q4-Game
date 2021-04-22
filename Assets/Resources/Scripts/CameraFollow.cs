using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followObj;
    private Vector3 offset;
    public float speed = 5f;

    public static Vector3 defaultOffset = new Vector3(0f, 0f, -12.5f);
    public static Vector3 lookUpOffset = new Vector3(0f, 2.5f, -12.5f);

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, followObj.position + offset, Time.deltaTime * speed);
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
