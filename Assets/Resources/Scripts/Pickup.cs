using UnityEngine;

public class Pickup : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private BoxCollider2D boxCollider;
    [HideInInspector] public GameObject carrier;

    [SerializeField] private bool showDebugs;
    private LayerMask player;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        player = LayerMask.NameToLayer("Player");
    }

    private void Update()
    {
        if (carrier != null)
        {
            transform.position = carrier.transform.position;
        }
    }

    public void Grab(GameObject carrier, out bool isSuccessful)
    {
        if (!CheckForPlayer(Vector2.zero, new Vector2(1f, 1f)))
            return;
    
        this.carrier = carrier;

        isSuccessful = true;
        boxCollider.enabled = false;
    }

    public void Throw(Vector2 throwVec)
    {
        this.carrier = null;

        rb2D.AddForce(throwVec);
    }

    private bool CheckForPlayer(Vector2 boxOffset, Vector2 boxSize)
    {
        RaycastHit2D hit = Physics2D.BoxCast((Vector2) transform.position - boxOffset, boxSize, 0f, Vector2.down, 0f, player);
        bool isPlayer = hit;
        
        if (showDebugs)
        {
            Debug.Log("Player detected.");
        }
        
        return isPlayer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.carrier != null)
            return;

        if (collision.CompareTag("Ground"))
        {
            boxCollider.enabled = true;
        }

        if (showDebugs)
        {
            Debug.Log("Hit: " + collision.gameObject);
        }
    }
}
