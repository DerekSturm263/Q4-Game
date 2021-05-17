using UnityEngine;

public class DarkZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement.isInDarkZone = true;
            LightsController.strength = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement.isInDarkZone = false;
            LightsController.strength = 1f;
        }
    }
}
