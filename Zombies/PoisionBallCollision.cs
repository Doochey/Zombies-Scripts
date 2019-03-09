using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PoisionBallCollision : MonoBehaviour
{
    private float damage;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player")) // If player collides with poison ball
        {
            // Take initial damage
            other.GetComponent<PlayerHealth>().takeDamage(damage, gameObject.tag);
            
            // Begin poison damage
            other.GetComponent<PlayerHealth>().takePoison();
            
            //Destroy poison ball
            Destroy(gameObject);
        }
    }
    
    public void setDamage(float dam)
    {
        damage = dam;
    }
}
