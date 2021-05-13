[System.Serializable]
public class AbilitySaveData
{
    public bool hasUsed;

    public AbilitySaveData(UnityEngine.GameObject ability)
    {
        hasUsed = !ability.activeSelf;
    }
}
