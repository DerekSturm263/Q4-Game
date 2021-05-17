using UnityEngine;

public class DarkZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement.isInDarkZone = true;

            if ((PlayerMovement.abilities & PlayerMovement.nightVision) == 0)
            {
                LightsController.targetStrength = 0f;
            }
            else
            {
                if (UIController.timeTitle == "night")
                {
                    LightsController.targetStrength = 0.8f;
                }
                else
                {
                    LightsController.targetStrength = 0.25f;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement.isInDarkZone = false;
            LightsController.targetStrength = 1f;
        }
    }
}
