using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{

    public float HP = 5f;

    private bool added;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") && !added)
        {
            if (other.gameObject.GetComponent<PlayerHealth>().getHealth() < 125)
            {
                other.gameObject.GetComponent<PlayerHealth>().increaseHealth(HP);
                added = true;
                GameObject.FindWithTag("GM").GetComponent<GameMaster>().subtractDroppable();
                GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addHealthPacksPickedUp();
                Destroy(gameObject);
            }
            
            
        }
    }
}
