using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followObj;

    private void Update()
    {
        transform.position = followObj.position + new Vector3(0f, 0f, -10f);
    }
}
