using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntivenomPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            if (other.gameObject.GetComponent<PlayerHealth>().isPoisoned())
            {
                other.gameObject.GetComponent<PlayerHealth>().stopPoison();
                GameObject.FindWithTag("GM").GetComponent<GameMaster>().subtractDroppable();
                GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addAV();
                Destroy(gameObject);
            }
           
        }
    }
}
