using UnityEngine;

public class CameraSaveData
{
    public float[] position;

    public CameraSaveData(Camera cam)
    {
        position[0] = cam.transform.position.x;
        position[1] = cam.transform.position.y;
    }
}
