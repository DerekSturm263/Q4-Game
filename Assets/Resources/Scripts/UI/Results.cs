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
        completionTime.text = (int) UIController.timePassedSinceGameBegun % 60 + ":" + (int) UIController.timePassedSinceGameBegun / 60;
        daysPassed.text = System.Convert.ToString(UIController.numDays);
        berriesCollected.text = CollectBerries.berriesCollectedNum + "/" + CollectBerries.totalBerries;
        deathCount.text = System.Convert.ToString(PlayerMovement.deathCount);
    }

    public void ToCredits()
    {
        fadeOut.SetTrigger("Exit");
    }
}
