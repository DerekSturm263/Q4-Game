[System.Serializable]
public class EntitySaveData
{
    public float[] position = new float[2];
    public bool isSatisified;

    public EntitySaveData(EntityAI entity)
    {
        this.position[0] = entity.transform.position.x;
        this.position[1] = entity.transform.position.y;

        this.isSatisified = entity.isSatisfied;
    }
}
