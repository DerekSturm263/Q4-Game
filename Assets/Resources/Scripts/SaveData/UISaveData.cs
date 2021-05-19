[System.Serializable]
public class UISaveData
{
    public float time;
    public string timeTitle;

    public float berryCount;
    public float sunRotation;

    public int daysPassed;
    public float timePassed;

    public UISaveData(UIController cont)
    {
        this.time = UIController.time;
        this.timeTitle = UIController.timeTitle;
        
        this.berryCount = UIController.numFood;
        this.sunRotation = cont.timeDisplay.transform.rotation.eulerAngles.z;
        
        this.daysPassed = UIController.numDays;
        this.timePassed = UIController.timePassedSinceGameBegun;
    }
}
