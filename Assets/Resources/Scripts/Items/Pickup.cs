using UnityEngine;

public class Pickup : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private PolygonCollider2D col;
    private SpriteRenderer sprtRndr;
    private AudioSource audioSrc;

    private ParticleSystem particles;
    private ParticleSystem.EmissionModule emission;

    private ParticleSystem waterSplashParticles;

    private SpriteRenderer carrierSprtRndr;

    [SerializeField] private float underwaterGravity = 0.5f;
    private float aboveWaterGravity;

    [HideInInspector] public GameObject carrier;
    public float weight = 1f;

    [SerializeField] private bool showDebugs;
    [HideInInspector] public float timeSinceGrabbed;

    [HideInInspector] public Vector2 outsideVel;

    public float maxYVelocity = 15f;
    [HideInInspector] public float defaultMaxYVelocity;

    [HideInInspector] public Vector2 offset;

    [SerializeField] public AudioClip[] landOnGround = new AudioClip[3];
    [SerializeField] private AudioClip[] waterSplash = new AudioClip[3];

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        col = GetComponent<PolygonCollider2D>();
        sprtRndr = GetComponent<SpriteRenderer>();
        audioSrc = GetComponent<AudioSource>();

        particles = GetComponentsInChildren<ParticleSystem>()[0];
        waterSplashParticles = GetComponentsInChildren<ParticleSystem>()[1];
        emission = particles.emission;

        aboveWaterGravity = rb2D.gravityScale;
    }

    private void Update()
    {
        if (carrier == null)
        {
            if (outsideVel.magnitude > 0.2f)
            {
                rb2D.velocity = Vector2.Lerp(rb2D.velocity, outsideVel / weight * 1.475f, Time.deltaTime * 5f);
            }
            return;
        }

        sprtRndr.flipX = carrierSprtRndr.flipX;

        timeSinceGrabbed += Time.deltaTime;
        transform.rotation = Quaternion.identity;
        transform.position = Vector2.Lerp(transform.position, (Vector2) carrier.transform.position + offset * (carrierSprtRndr.flipX ? -1f : 1f), timeSinceGrabbed);
    }

    public void Grab(GameObject carrier)
    {
        this.carrier = carrier;
        emission.rateOverTime = 0f;

        if (carrier.GetComponent<PlayerMovement>() != null)
        {
            offset = carrier.GetComponent<PlayerMovement>().carrySpot;
        }
        else
        {
            offset = carrier.GetComponent<EntityAI>().carrySpot;
        }
        carrierSprtRndr = carrier.GetComponent<SpriteRenderer>();
        timeSinceGrabbed = 0f;
        col.enabled = false;
        rb2D.gravityScale = 0f;
        rb2D.velocity = Vector2.zero;
    }

    public void Throw(Vector2 throwVec)
    {
        carrier = null;
        emission.rateOverTime = 10f;

        carrierSprtRndr = null;
        timeSinceGrabbed = 0f;
        col.enabled = true;
        rb2D.gravityScale = aboveWaterGravity;
        rb2D.velocity = Vector2.zero;

        rb2D.AddForce(throwVec / weight, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            if (showDebugs)
            {
                Debug.Log(name + " Entered Water");
            }

            waterSplashParticles.Play();
            PlaySound(waterSplash, true, volume: weight);
            EnterWater();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            if (showDebugs)
            {
                Debug.Log(name + " Exited Water");
            }

            waterSplashParticles.Play();
            PlaySound(waterSplash, true, volume: weight);
            ExitWater();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            PlaySound(landOnGround, true, volume: weight * 2f);
        }
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

    private void EnterWater()
    {
        rb2D.velocity = new Vector2(rb2D.velocity.x / 2f, rb2D.velocity.y / 10f);
        rb2D.gravityScale = underwaterGravity;
    }

    private void ExitWater()
    {
        rb2D.gravityScale = aboveWaterGravity;
    }
}
