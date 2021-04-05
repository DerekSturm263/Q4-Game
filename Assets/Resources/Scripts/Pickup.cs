using UnityEngine;

public class Pickup : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private BoxCollider2D boxCollider;
    [HideInInspector] public GameObject carrier;

    [SerializeField] private bool showDebugs;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (carrier != null)
        {
            transform.position = carrier.transform.position;
        }
    }

    public void Grab(GameObject carrier)
    {
        this.carrier = carrier;

        boxCollider.enabled = false;
    }

    public void Throw(Vector2 throwVec)
    {
        this.carrier = null;

        rb2D.AddForce(throwVec);
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
