[System.Serializable]
public class CameraSaveData
{
    public float[] position = new float[3];

    public CameraSaveData(UnityEngine.Camera cam)
    {
        this.position[0] = cam.transform.position.x;
        this.position[1] = cam.transform.position.y;
        this.position[2] = cam.transform.position.z;
    }
}
