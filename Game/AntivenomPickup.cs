using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntivenomPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player")) // If player walks over
        {
            if (other.gameObject.GetComponent<PlayerHealth>().isPoisoned()) // iff player is poisoned
            {
                // Stop the player from taking poison damage
                other.gameObject.GetComponent<PlayerHealth>().stopPoison();
                
                // Subtract droppable from world droppables counter
                GameObject.FindWithTag("GM").GetComponent<GameMaster>().subtractDroppable();
                
                // Log AV pickup action
                GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addAV();
                
                // Destroy the AV
                Destroy(gameObject);
            }
           
        }
    }
}
