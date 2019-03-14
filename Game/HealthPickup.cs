using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{

    public int HP = 5;

    private bool added;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") && !added) // If player walks over health pack
        {
            // If player health less than max
            if (other.gameObject.GetComponent<PlayerHealth>().getHealth() < 125)
            {
                
                // Increase health by health pack ampount
                other.gameObject.GetComponent<PlayerHealth>().increaseHealth(HP);
                added = true;
                
                
                // Log health pack picked up
                GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addHealthPacksPickedUp();
                
                // Destroy health pack
                Destroy(gameObject);
            }
            
            
        }
    }

    private void OnDestroy()
    {
        // Subtract droppable from world counter
        GameObject.FindWithTag("GM").GetComponent<GameMaster>().subtractDroppable();
    }
}
