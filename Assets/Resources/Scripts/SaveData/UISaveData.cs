[System.Serializable]
public class UISaveData
{
    public float berryCount;
    public float time;

    public UISaveData(UIController cont)
    {
        berryCount = UIController.numFood;
        time = UIController.time;
    }
}
