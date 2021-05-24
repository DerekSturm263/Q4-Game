using UnityEngine;

public class Cutscene : MonoBehaviour
{
    private PlayerMovement player;

    public GameObject newTarget;
    private float timeSinceCutscene = -1f;

    private void Update()
    {
        if (timeSinceCutscene > 0f)
        {
            timeSinceCutscene -= Time.deltaTime;
        }
        else if (timeSinceCutscene > -1f && timeSinceCutscene <= 0f)
        {
            player.EndCutscene();
            PlayerMovement.UnFreeze();
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement.Freeze();
            player = collision.GetComponent<PlayerMovement>();
            player.StartCutscene(newTarget);

            timeSinceCutscene = 3f;
        }
    }
}
