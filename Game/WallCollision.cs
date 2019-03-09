using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    // If bullet hits wall, destroys bullet
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("poison"))
        {
            Destroy(other.gameObject);
        }
    }
}
