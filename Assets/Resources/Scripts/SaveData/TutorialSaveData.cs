[System.Serializable]
public class TutorialSaveData
{
    public bool hasUsed;

    public TutorialSaveData(UnityEngine.GameObject tutorial)
    {
        hasUsed = !tutorial.activeSelf;
    }
}
