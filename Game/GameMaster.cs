using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameMaster : MonoBehaviour
{
    /*
     * Controls the flow of the game
     * Spawns enemies and items
     * Tracks enemies alive and wave number
     * ends game when player dead or wave limit reached
     */
    
    public float skill; // Skill of player
    public GameObject[] spawnPoints; // List of possible spawnpoints
    public Stack<GameObject> waveComp; // Stack of Zs for upcoming wave
    public GameObject[] enemyList; // List of possible enemy types
    public int zLimit = 60; // Max number of Zs
    public float spawnRate = 2f; // Rate Zs spawn
    public int maxDroppables; // Max number of items in the world at once
    public int maxWaves; // Max number of waves player can play
    
    public GameObject smallHealth; // Small Health Pack
    public GameObject mediumHealth; // Medium Health Pack
    public GameObject largeHealth; // Large Health pack
    public GameObject antiVenom; // Anti Venom
    public GameObject healthSpawner; // Health Spawner ~At center of map

    public GameObject player;
    public float minSpawnDistance;
    
    
    
    private float waveDR;  // Difficulty Rating of Wave
    private bool gameOver = false; 
    private int wave = 0;
    private bool inWave = false;
    private float zLeft = 0f; // Zs Alive in current wave
    private int spawned = 0; // Zs successfully spawned, for Debugging
    private int toSpawn = 0; // Number of Zs that should spawn
    private string lastEnemyToAttack; // Last enemy to do damage to player
    private Text waveNumber; // Text showing wave number
    private Text zombieNumber; // Text showing Zs left alive
    private StatLogging logger;


    private static int currentDroppables = 0; // Current number of items in the world
    
    

    
    void Start()
    {
        
        waveComp = new Stack<GameObject>();
        waveNumber = GameObject.FindWithTag("wave").GetComponent<Text>();
        zombieNumber = GameObject.FindWithTag("zleft").GetComponent<Text>();
        logger = GameObject.FindWithTag("StatLog").GetComponent<StatLogging>();
    }

    void FixedUpdate()
    {
        zombieNumber.text = "Zs LEFT: " + zLeft;
        
        // If game is not over and wave not in progress
        if (!gameOver && !inWave && wave < 10)
        {
            wave++;
            
            ManageEnemyList();
            
            waveNumber.text = "WAVE: " + wave;
            waveDR = 50 * wave + skill;
            Debug.Log("Wave Difficulty: " + waveDR);
            // Build composition of wave
            loadWaveComposition();
            
            // Count number of Z that should be spawned
            toSpawn = waveComp.Count;
            spawned = 0;
            
            // Wave in progress
            inWave = true;
            
            // Spawn Z every spawnRate seconds.
            // Number of Zs spawning at once proportional to wave number
            for (int i = 0; i < wave; i++)
            {
                StartCoroutine(spawnZ());
            }
            
            
            
        }
        
        if (spawned == toSpawn) // If all Z spawned
        {
            // Stop trying to spawn
            StopAllCoroutines();
        }

        
        if (zLeft <= 0) // If all Z dead
        {
            
            //Wave no longer in progress
            inWave = false;

            // Spawn Health
            SpawnHealth();
            
            // Set next wave DR
            waveDR = 50 * wave;
        }

        // If max waves reached
        if (wave >= maxWaves)
        {
            gameOver = true;
            
        }

        if (gameOver)
        {
            StopAllCoroutines();
            logger.SetKilledPlayer(lastEnemyToAttack);
            logger.addWaveReached(wave);
            logger.StatDump();
        }
        
        
    }


    public bool isGameOver()
    {
        return gameOver;
    }

    public void setGameOver()
    {
        gameOver = true;
    }

    /* Select random Z from all possible
     * Calculate 'trueDR'
     * If trueDR is less than maximum and wave not full, add Z to upcoming wave stack
    */
    void loadWaveComposition()
    {
        zLimit = 20 * wave;
        bool waveCompDone = false;
        int cycles = 0; // Possible that waveDR left is not enough for any Z to be added to wave
        while (!waveCompDone)
        {
            
            // Select random enemy from possible
            int enemyIndex = Random.Range(0, enemyList.Length);
            
            // Calculate 'true' difficulty rating, DR of Z / Player skill
            float trueDR = enemyList[enemyIndex].GetComponent<ZombieAI>().DR / skill;
            
            if (trueDR < waveDR && waveComp.Count < zLimit) // If trueDR is below wave max and wave not at max Zs
            {
                // Add Z to wave
                waveComp.Push(enemyList[enemyIndex]);
                logger.addWaveDR(trueDR);

                // Subtract Z trueDR from wave total
                waveDR -= trueDR;
            }
            
            if (waveComp.Count == zLimit || cycles >= zLimit) // If max Zs reached or max Cycles
            {
                waveCompDone = true;
                Debug.Log("Wave Comp Loaded");
                Debug.Log("WaveComp Count: " + waveComp.Count);
            }
            else
            {
                cycles++;
            }
        }

        zLeft = waveComp.Count;
    }

    // Spawns a Z from wave to random spawnpoint
    private IEnumerator spawnZ()
    {
        while(waveComp.Count != 0) // If Zs left in stack
        {
            
            // Get Z from waveComp
            GameObject Z = waveComp.Pop();
            
            // Select random spawnpoint from list
            int spawnPointIndex = Random.Range(0, spawnPoints.Length);
            GameObject spawnPoint = spawnPoints[spawnPointIndex];

            bool playerTooClose = Vector3.Distance(spawnPoint.transform.position, player.transform.position) < minSpawnDistance;

            // If spawn point is clear spawn Z
            if (!spawnPoint.GetComponentInChildren<SpawnerOccupied>().occupied && !playerTooClose)
            {
                Instantiate(Z, spawnPoint.transform.position, spawnPoint.transform.rotation);
                spawned++;
            }
            else
            {
                // If failed to spawn Z, push back onto stack and try again
                waveComp.Push(Z);
            }
            
            yield return new WaitForSeconds(spawnRate);
        }
        
    }
    
    // When Z health reaches 0, Z has chance of dropping item
    public void drop(GameObject Z)
    {
        // Random chance of dropping item
        bool dropOK = currentDroppables < maxDroppables;
        if (dropOK)
        {
            // Each Z only gets one dice roll
            int attempt = Random.Range(0, 4);
            switch (attempt)
            {
                case 0:
                    if (Random.Range(0, 10) == 1) // 10% chance of small Health Pack
                    {
                        currentDroppables++;
                        GameObject droppable = Instantiate(smallHealth, Z.transform.position, Z.transform.rotation);
                        Destroy(droppable, 30f);
                    }
                    break;
                case 1:
                    if (Random.Range(0, 20) == 1) // 5% chance of medium Health Pack
                    {
                        currentDroppables++;
                        GameObject droppable = Instantiate(mediumHealth, Z.transform.position, Z.transform.rotation);
                        Destroy(droppable, 30f);
                    }
                    break;
                case 2:
                    if (Random.Range(0, 100) == 1) // 1% chance of large Health Pack
                    {
                        currentDroppables++;
                        GameObject droppable = Instantiate(largeHealth, Z.transform.position, Z.transform.rotation);
                        Destroy(droppable, 30f);
                    }
                    break;
                case 3:
                    if (Random.Range(0, 20) == 1) // 10% chance of anti Venom
                    {
                        currentDroppables++;
                        GameObject droppable = Instantiate(antiVenom, Z.transform.position, Z.transform.rotation);
                        Destroy(droppable, 30f);
                    } 
                    break;
                    
            }
        }
    }

    // Spawns health at health spawner. Size of health pack depends on wave number
    private void SpawnHealth()
    {
        if (currentDroppables < maxDroppables && wave % 2 > 0)
        {
            if (wave < 4)
            {
                Instantiate(smallHealth, healthSpawner.transform.position, healthSpawner.transform.rotation);
            
            }
            else if (wave < 8)
            {
                Instantiate(mediumHealth, healthSpawner.transform.position, healthSpawner.transform.rotation);
            }
            else
            {
                Instantiate(largeHealth, healthSpawner.transform.position, healthSpawner.transform.rotation);
            }
            
            currentDroppables++;
        }
        
    }

    // Register Z has been killed
    public void zDead()
    {
        zLeft-=1;
        logger.addZKilled();
    }

    // Record the last enemy to do damage to the player
    public void recordLastEnemyToAttack(string tag)
    {
        Debug.Log(tag);
        lastEnemyToAttack = tag;
    }

    // Add Z to Z count ( Used for arachnid's spawn minion ability )
    public void addZ()
    {
        zLeft++;
    }

    // When a droppable is picked up by the player it is no longer in the world
    public void subtractDroppable()
    {
        currentDroppables--;
    }

    private void ManageEnemyList()
    {
        switch (wave)
        {
            case 1:
                enemyList = GetComponentInChildren<PossibleEnemies>().tutorialList;
                break;
            case 2:
                enemyList = GetComponentInChildren<PossibleEnemies>().veryEasyList;
                break;
            case 3:
            case 4:
                enemyList = GetComponentInChildren<PossibleEnemies>().easyList;
                break;    
            case 5:
            case 6:
                enemyList = GetComponentInChildren<PossibleEnemies>().mediumList;
                break;
            case 7:
                enemyList = GetComponentInChildren<PossibleEnemies>().hardList;
                break;
            case 8:
                enemyList = GetComponentInChildren<PossibleEnemies>().veryHardList;
                break;
            case 9:
                enemyList = GetComponentInChildren<PossibleEnemies>().extremeList;
                break;
            case 10:
                enemyList = GetComponentInChildren<PossibleEnemies>().insaneList;
                break;
        }
    }

    public GameObject getPlayer()
    {
        return player;
    }
}
