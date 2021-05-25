[System.Serializable]
public class UISaveData
{
    public float time;
    public string timeTitle;

    public float berryCount;
    public float sunRotation;

    public int daysPassed;
    public float timePassed;

    public bool useFullscreen;
    public bool useParticles;
    public bool usePostProcessing;
    public float musicVolume;
    public float sfxVolume;

    public UISaveData(UIController cont)
    {
        this.time = UIController.time;
        this.timeTitle = UIController.timeTitle;
        
        this.berryCount = UIController.numFood;
        this.sunRotation = cont.timeDisplay.transform.rotation.eulerAngles.z;
        
        this.daysPassed = UIController.numDays;
        this.timePassed = UIController.timePassedSinceGameBegun;

        this.useFullscreen = Settings.useFullscreen;
        this.useParticles = Settings.useParticles;
        this.usePostProcessing = Settings.usePostProcessing;
        this.musicVolume = MusicPlayer.volume[0];
        this.sfxVolume = SoundPlayer.volume;
    }
}
