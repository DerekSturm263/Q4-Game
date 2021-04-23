using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBerry : MonoBehaviour
{
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "UIBerry")
            Destroy(collision.gameObject);
    }
}
