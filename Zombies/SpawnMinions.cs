using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMinions : MonoBehaviour
{
    // Spawn minions on Z death
    
    
    public GameObject littleZ; // The prefab to spawn
    public bool doneSpawning = false; // If all minions spawned
    public int maxMinions = 5; // max minions to spawn

    private int minionsSpawned = 0; // Number spawned so far

    

    public IEnumerator spawnMinion()
    {
        // While number spawned not at maximum
        while (minionsSpawned < maxMinions)
        {
            // Spawn minion
            GameObject Z = Instantiate(littleZ, transform.position, transform.rotation);
            minionsSpawned++;
            
            // Add Z to wave count
            GameObject.FindWithTag("GM").GetComponent<GameMaster>().addZ();
            
            yield return new WaitForSecondsRealtime(1f);
        }

        doneSpawning = true;



    }
}
