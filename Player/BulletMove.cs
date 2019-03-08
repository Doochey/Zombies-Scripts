using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{

    private Vector3 point;
    private float bulletSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        
        InvokeRepeating("bulletMove", 0f, bulletSpeed);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }
    
    

    void bulletMove()
    {
        if (transform.position != point)
        {
            transform.position = Vector3.Lerp(transform.position, point, 0.1f);
        }
        else
        {
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
