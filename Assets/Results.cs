using UnityEngine;

public class Results : MonoBehaviour
{
    public TMPro.TMP_Text completionTime;
    public TMPro.TMP_Text daysPassed;
    public TMPro.TMP_Text berriesCollected;
    public TMPro.TMP_Text deathCount;

    private void Awake()
    {
        completionTime.text = "0:00";
        daysPassed.text = "0";
        berriesCollected.text = CollectBerries.berriesCollectedNum + "/" + CollectBerries.totalBerries;
        deathCount.text = System.Convert.ToString(PlayerMovement.deathCount);
    }
}
