using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerOccupied : MonoBehaviour
{
    public bool occupied = false; // If spawner is blocked

    private void OnTriggerStay(Collider other)
    {
        // If another object on top of spawner, spawner is 'occupied'
        occupied = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // When object leaves spawner collider, no longer occupied
        occupied = false;
    }
}
