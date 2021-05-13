using UnityEngine;

public class AbilityUnlock : MonoBehaviour
{
    [SerializeField] private byte ability;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement.UnlockAbility(ability);
            LoadTutorial.Display(AbilityTutorial.abilities[ability]);
            SoundPlayer.Play("new_ability");

            gameObject.SetActive(false);
        }
    }
}
