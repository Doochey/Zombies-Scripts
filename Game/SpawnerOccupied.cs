using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerOccupied : MonoBehaviour
{
    public bool occupied = false;

    private void OnTriggerStay(Collider other)
    {
        occupied = true;
    }

    private void OnTriggerExit(Collider other)
    {
        occupied = false;
    }
}
