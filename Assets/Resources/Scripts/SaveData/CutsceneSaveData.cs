[System.Serializable]
public class CutsceneSaveData
{
    public bool enabled;

    public CutsceneSaveData(UnityEngine.GameObject obj)
    {
        this.enabled = obj.activeSelf;
    }
}
