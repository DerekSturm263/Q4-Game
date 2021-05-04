[System.Serializable]
public class PickupSaveData
{
    public float[] position = new float[2];

    public PickupSaveData(Pickup pickUp)
    {
        this.position[0] = pickUp.transform.position.x;
        this.position[1] = pickUp.transform.position.y;
    }
}
