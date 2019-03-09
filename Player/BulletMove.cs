using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    // Bullet moves towards point by 1/10 the distance
    /* BUG: bullet will never reach point. Tried despawning at certain distance but resulted in Zs not being hit from behind
     * This method reduced bug where bullet speed is increased with distance 
    */

    private Vector3 point; // Destination of bullet
    private float bulletSpeed; // Speed of bullet
    
    // Start is called before the first frame update
    void Start()
    {
        // Moves bullet by set distance every bulletSpeed seconds
        InvokeRepeating("bulletMove", 0f, bulletSpeed);
        
    }

    

    void bulletMove()
    {
        // If bullet not at destination
        if (transform.position != point)
        {
            // Position of bullet = 1/10 of the distance between bullet and destination
            transform.position = Vector3.Lerp(transform.position, point, 0.1f);
        }
        else // If bullet at point
        {
            // Destroy bullet
            Destroy(gameObject);
        }
        
    }

    public void setPoint(Vector3 point)
    {
        this.point = point;
    }

    public void setSpeed(float speed)
    {
        this.bulletSpeed = speed;
    }
}
