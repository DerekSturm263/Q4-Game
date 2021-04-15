using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followObj;
    public Vector3 offset;
    public float speed = 5f;

    private void Awake()
    {
        offset = transform.position - followObj.transform.position;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, followObj.position + offset, Time.deltaTime * speed);
    }
}
