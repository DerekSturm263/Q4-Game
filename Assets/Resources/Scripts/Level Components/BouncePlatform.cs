using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSrc;

    public float bounceForce;

    [SerializeField] private AudioClip[] bounce = new AudioClip[3];

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb2D = collision.gameObject.GetComponent<Rigidbody2D>();

        if (rb2D.velocity.y > 0f)
            return;

        rb2D.velocity = new Vector2(rb2D.velocity.x, bounceForce);
        PlaySound(bounce, true);
        anim.SetTrigger("Bounce");
    }

    public void PlaySound(AudioClip[] sound, bool interuptSound = false, float pitch = 1f, float volume = 1f)
    {
        if (audioSrc.isPlaying && !interuptSound)
            return;

        audioSrc.clip = sound[Random.Range(0, sound.Length)];
        audioSrc.pitch = pitch;
        audioSrc.volume = volume;
        audioSrc.Play();
    }
}
