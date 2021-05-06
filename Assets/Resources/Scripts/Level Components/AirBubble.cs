using UnityEngine;

public class AirBubble : MonoBehaviour
{
    private ParticleSystem ps;
    private Animator anim;

    [SerializeField] private float respawnTime = 10f;
    [HideInInspector] public float timeSincePopped;

    private Vector2 posUp, posDown;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        anim = GetComponent<Animator>();

        posUp = (Vector2) transform.position + Vector2.up / 2f;
        posDown = (Vector2) transform.position + Vector2.down / 2f;
    }

    private void Update()
    {
        transform.position = Vector2.Lerp(posUp, posDown, Mathf.Sin(Time.realtimeSinceStartup) / 2f + 0.5f);

        if (timeSincePopped > 0f)
        {
            timeSincePopped += Time.deltaTime;
        }

        if (timeSincePopped >= respawnTime)
        {
            Respawn();
            timeSincePopped = 0f;
        }
    }

    private void Respawn()
    {
        anim.SetTrigger("Respawn");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        timeSincePopped = 0.01f;

        anim.SetTrigger("Pop");
        player.RestoreBreath();
        ps.Play();
    }
}
