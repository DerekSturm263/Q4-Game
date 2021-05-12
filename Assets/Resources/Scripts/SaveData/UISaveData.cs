[System.Serializable]
public class UISaveData
{
    public float time;
    public string timeTitle;

    public float berryCount;
    public float sunRotation;

    public UISaveData(UIController cont)
    {
        time = UIController.time;
        timeTitle = UIController.timeTitle;

        berryCount = UIController.numFood;
        sunRotation = cont.timeDisplay.transform.rotation.eulerAngles.z;
    }
}
