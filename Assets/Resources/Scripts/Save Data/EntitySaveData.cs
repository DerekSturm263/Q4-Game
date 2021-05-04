using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySaveData : MonoBehaviour
{
    public float[] position;
    public bool isSatisified;

    public EntitySaveData(EntityAI entity)
    {
        position[0] = entity.transform.position.x;
        position[1] = entity.transform.position.y;

        isSatisified = entity.isSatisfied;
    }
}
