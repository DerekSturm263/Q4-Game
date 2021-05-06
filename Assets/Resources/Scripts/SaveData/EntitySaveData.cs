[System.Serializable]
public class EntitySaveData
{
    public float[] position = new float[2];
    public bool isSatisified;
    public int itemCarried; // 0 means nothing. 1 and up pairs to items in the level.

    public EntitySaveData(EntityAI entity)
    {
        this.position[0] = entity.transform.position.x;
        this.position[1] = entity.transform.position.y;

        this.isSatisified = entity.isSatisfied;
        this.itemCarried = entity.pickupNum;
    }
}
