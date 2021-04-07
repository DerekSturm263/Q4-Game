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
        carrier = null;

        boxCollider.enabled = true;
        rb2D.AddForce(throwVec, ForceMode2D.Impulse);
    }
}
