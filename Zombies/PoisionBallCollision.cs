using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisionBallCollision : MonoBehaviour
{
    private float damage;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            other.GetComponent<PlayerHealth>().takeDamage(damage, gameObject.tag);
            other.GetComponent<PlayerHealth>().takePoison();
            Destroy(gameObject);
        }
    }
    
    public void setDamage(float dam)
    {
        damage = dam;
    }
}
