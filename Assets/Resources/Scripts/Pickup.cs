using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Rigidbody2D rb2D;
    private BoxCollider2D boxCollider;

    [SerializeField] private float underwaterGravity = 0.5f;
    private float aboveWaterGravity;

    [HideInInspector] public GameObject carrier;
    public Vector2 offset;
    public float weight = 1f;

    [SerializeField] private bool showDebugs;
    [HideInInspector] public float timeSinceGrabbed;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        aboveWaterGravity = rb2D.gravityScale;
    }

    private void Update()
    {
        if (carrier == null)
            return;

        timeSinceGrabbed += Time.deltaTime;
        transform.position = Vector2.Lerp(transform.position, (Vector2) carrier.transform.position + offset, timeSinceGrabbed);
    }

    public void Grab(GameObject carrier)
    {
        this.carrier = carrier;

        timeSinceGrabbed = 0f;
        boxCollider.enabled = false;
        rb2D.gravityScale = 0f;
        rb2D.velocity = Vector2.zero;
    }

    public void Throw(Vector2 throwVec)
    {
        carrier = null;

        timeSinceGrabbed = 0f;
        boxCollider.enabled = true;
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

            ExitWater();
        }
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
