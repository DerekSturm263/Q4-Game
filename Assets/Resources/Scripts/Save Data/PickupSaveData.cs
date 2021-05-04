using UnityEngine;

public class PickupSaveData
{
    public float[] position;

    public PickupSaveData(Pickup pickUp)
    {
        position[0] = pickUp.transform.position.x;
        position[1] = pickUp.transform.position.y;
    }
}
