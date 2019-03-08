using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMinions : MonoBehaviour
{
    public GameObject littleZ;
    public bool doneSpawning = false;
    public int maxMinions = 5;

    private int minionsSpawned = 0;

    

    public IEnumerator spawnMinion()
    {
        
        while (minionsSpawned < maxMinions)
        {
            GameObject Z = Instantiate(littleZ, transform.position, transform.rotation);
            minionsSpawned++;
            GameObject.FindWithTag("GM").GetComponent<GameMaster>().addZ();
            yield return new WaitForSecondsRealtime(1f);
        }

        doneSpawning = true;



    }
}
