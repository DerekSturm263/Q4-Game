using UnityEngine;

public class Results : MonoBehaviour
{
    public TMPro.TMP_Text completionTime;
    public TMPro.TMP_Text daysPassed;
    public TMPro.TMP_Text berriesCollected;
    public TMPro.TMP_Text deathCount;

    public Animator fadeOut;

    private void Awake()
    {
        completionTime.text = ((int) UIController.timePassedSinceGameBegun / 60).ToString().PadLeft(2, '0') + ":" + ((int) UIController.timePassedSinceGameBegun % 60).ToString().PadLeft(2, '0');
        daysPassed.text = System.Convert.ToString(UIController.numDays);
        berriesCollected.text = CollectBerries.berriesCollectedNum + "/60";
        deathCount.text = System.Convert.ToString(PlayerMovement.deathCount);
    }

    public void ToCredits()
    {
        SoundPlayer.Play("ui_select");
        fadeOut.SetTrigger("Exit");
    }
}
