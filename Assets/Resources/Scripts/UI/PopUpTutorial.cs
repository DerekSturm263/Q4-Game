using UnityEngine;

public class PopUpTutorial : MonoBehaviour
{
    public string label;
    [Multiline] public string description;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadTutorial.Display(label, description);
            gameObject.SetActive(false);
            SoundPlayer.Play("berry_picked");
        }
    }
}
