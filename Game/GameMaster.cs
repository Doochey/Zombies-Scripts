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
    
    
    public float spawnRate = 2f; // Rate Zs spawn
    public float minSpawnDistance; // Minimum distance spawner must be from player to spawn Z
    public float skill; // Skill of player
    
    public GameObject[] spawnPoints; // List of possible spawnpoints
    public GameObject[] enemyList; // List of possible enemy types
    
    public GameObject smallHealth; // Small Health Pack
    public GameObject mediumHealth; // Medium Health Pack
    public GameObject largeHealth; // Large Health pack
    public GameObject antiVenom; // Anti Venom
    public GameObject healthSpawner; // Health spawner ~At center of map
    public GameObject player; // The player
    public GameObject gameOverMenu; // Menu for displaying at game over (defeat)
    public GameObject winMenu; // Menu for displaying at game over (win)

    public Stack<GameObject> waveComp; // Stack of Zs for upcoming wave
    
    public int maxDroppables; // Max number of items in the world at once
    public int maxWaves; // Max number of waves player can play
    public int zLimitModifier; // Constant multiplied by wave to get max number of Zs
    public int waveDRConstant; // Constant offset for DR of wave. Higher == More difficult
    
    

    
    
    private float waveDR;  // Difficulty Rating of Wave
    private float zLeft = 0f; // Zs Alive in current wave
    
    private bool gameOver = false; // If game is over or not
    private bool inWave = false; // If in wave or not
    private bool win = false; // If game has been won
    private bool newZIntroduced = false;
    private int tier1Introduced = 0;
    private int tier2Introduced = 0;
    
    private int wave = 0; // Current wave number
    private int spawned = 0; // Zs successfully spawned, for Debugging
    private int toSpawn = 0; // Number of Zs that should spawn
    private int zLimit = 60; // Max number of Zs
    private int currentDroppables = 0; // Current number of items in the world. 
    private int stress = 0;
    private int ZAttacking = 0;
    private int currentWaveTrueDR = 0;
    private int ZAggroed;
    private int zAlive;
    
    private string lastEnemyToAttack; // Last enemy to do damage to player
    
    private Text waveNumber; // Text showing wave number
    private Text zombieNumber; // Text showing Zs left alive
    
    private StatLogging logger; // Stat Logger

    private ArrayList enemiesIntroduced;
    
    
    
    

    
    void Start()
    {
        
        waveComp = new Stack<GameObject>();
        waveNumber = GameObject.FindWithTag("wave").GetComponent<Text>();
        zombieNumber = GameObject.FindWithTag("zleft").GetComponent<Text>();
        logger = GameObject.FindWithTag("StatLog").GetComponent<StatLogging>();
        enemiesIntroduced = new ArrayList();
    }

    void FixedUpdate()
    {
        zombieNumber.text = "Zs LEFT: " + zLeft;
        
        // If game is not over and wave not in progress
        if (!gameOver && !inWave && wave < maxWaves)
        {
            // Inc Wave number
            wave++;
            
            // Adjust possible enemies, dependant on wave number
            ManageEnemyList();
            
            waveNumber.text = "WAVE: " + wave;
            
            // Calculate waveDR
            waveDR = waveDRConstant * wave + 5 * skill;
            
            // Log current wave difficulty
            
            // Build composition of wave
            loadWaveComposition();
            
            // Count number of Z that should be spawned
            toSpawn = waveComp.Count;
            spawned = 0;
            
            // Wave in progress
            inWave = true;
            
            // Spawn Z every spawnRate seconds.
            // Number of Zs spawning at once proportional to wave number. Higher wave == more Z spawn at once == More difficult
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
            

            // If final wave
            if (wave + 1 > maxWaves)
            {
                // Game Won
                win = true;
            }
            
        }

        
        if (win)
        {
            // Game is over (Win), display win menu
            // Should only rin when player not killed and won game
            
            gameOver = true;
            
            // Display win menu
            winMenu.SetActive(true);
            winMenu.GetComponent<Animator>().SetTrigger("GameOver");
            
            // Log game win
            logger.addGamesWon();
            
            logger.StatDump();
            
            // Reset cursor to default
            GetComponent<SetCursor>().ResetCursor();
            
        }
        else if (gameOver)
        {
            // Game is over (Defeat), Display game over menu
            // Should only run when player killed
            
            // Stop more Zs spawning
            StopAllCoroutines();
            
            // Log enemy that killed player
            logger.SetKilledPlayer(lastEnemyToAttack);
            
            // Log wave reached
            logger.addWaveReached(wave);
            // Print log to output
            logger.StatDump();
            
            // Display game over menu
            gameOverMenu.SetActive(true);
            gameOverMenu.GetComponent<Animator>().SetTrigger("GameOver");
            
            // Reset cursor to default
            GetComponent<SetCursor>().ResetCursor();
        }
        
        
    }


    public bool isGameOver()
    {
        return gameOver;
    }

    // Sets game over to true, used by player when health == 0
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
        // Calculate max Zs to spawn
        if (zLimit < 200)
        {
            zLimit = zLimitModifier * wave;
        }
        else
        {
            zLimit = 200;
        }
        
        bool waveCompDone = false;
        int cycles = 0; // Possible that waveDR left is not enough for any Z to be added to wave so limit looping
        float trueWaveDR = 0f;
        newZIntroduced = false;
        while (!waveCompDone)
        {
            
            // Select random enemy from all possible
            int enemyIndex = Random.Range(0, enemyList.Length);

            GameObject Z = enemyList[enemyIndex];
            
            
            

            if(OkToAdd(Z))
            {
                // Calculate 'true' difficulty rating, DR of Z / Player skill

                float trueDR;
                if (GameObject.FindWithTag("Zevolution").GetComponent<Zevolution>().IsActive())
                {
                    trueDR = Z.GetComponent<ModifiedDR>().GetDR(Z.tag) / skill;
                }
                else
                {
                    trueDR = Z.GetComponent<ZombieAI>().DR / skill;
                }
                
            
                if (trueDR < waveDR && waveComp.Count < zLimit) // If trueDR is below wave max and wave not at max Zs
                {
                    // Add Z to wave
                    waveComp.Push(Z);
                
                    trueWaveDR += trueDR;

                    // Subtract Z trueDR from wave total
                    waveDR -= trueDR;

                    currentWaveTrueDR += (int) trueDR;
                }
            
                if (waveComp.Count == zLimit || cycles >= zLimit*2) // If max Zs reached or max Cycles
                {
                    waveCompDone = true;
                }
                else
                {
                    cycles++;
                }
            }
            
            
        }
        logger.addWaveDR(trueWaveDR);

        // Update number of Zs alive
        zLeft = waveComp.Count;
    }

    private bool OkToAdd(GameObject Z)
    {
        bool okToAdd = false;
        if (GameObject.FindWithTag("Zevolution").GetComponent<Zevolution>().IsActive())
        {
            if (enemiesIntroduced.Contains(Z.gameObject.tag))
            {
                okToAdd = true;
            }
            else if (!newZIntroduced)
            {
                int tier = Z.GetComponent<ZombieAI>().tier;
                switch (tier)
                {
                    case 1:
                        tier1Introduced++;
                        okToAdd = true;
                        break;
                    case 2:
                        if (tier1Introduced == 3)
                        {
                            tier2Introduced++;
                            okToAdd = true;
                        }
                        break;
                    case 3:
                        if (tier1Introduced == 3 && tier2Introduced == 2)
                        {
                            okToAdd = true;
                        }
                        break;
                }

                if (okToAdd)
                {
                    enemiesIntroduced.Add(Z.gameObject.tag);
                    newZIntroduced = true;
                }
                
            }
        }
        else
        {
            okToAdd = true;
        }
        return okToAdd;
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

            // If player is within minimum distance or not
            bool playerTooClose = Vector3.Distance(spawnPoint.transform.position, player.transform.position) < minSpawnDistance;

            // If spawn point is clear spawn Z
            if (!spawnPoint.GetComponentInChildren<SpawnerOccupied>().occupied && !playerTooClose)
            {
                Instantiate(Z, spawnPoint.transform.position, spawnPoint.transform.rotation);
                spawned++;
                zAlive++;
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
        // If world droppables not at max capacity
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
                        
                        // Despawn droppable if not picked up after 30 seconds
                        Destroy(droppable, 30f);
                    }
                    break;
                case 1:
                    if (Random.Range(0, 20) == 1) // 5% chance of medium Health Pack
                    {
                        currentDroppables++;
                        GameObject droppable = Instantiate(mediumHealth, Z.transform.position, Z.transform.rotation);
                        
                        // Despawn droppable if not picked up after 30 seconds
                        Destroy(droppable, 30f);
                    }
                    break;
                case 2:
                    if (Random.Range(0, 100) == 1) // 1% chance of large Health Pack
                    {
                        currentDroppables++;
                        GameObject droppable = Instantiate(largeHealth, Z.transform.position, Z.transform.rotation);
                        
                        // Despawn droppable if not picked up after 30 seconds
                        Destroy(droppable, 30f);
                    }
                    break;
                case 3:
                    if (Random.Range(0, 20) == 1) // 10% chance of anti Venom
                    {
                        currentDroppables++;
                        GameObject droppable = Instantiate(antiVenom, Z.transform.position, Z.transform.rotation);
                        
                        // Despawn droppable if not picked up after 30 seconds
                        Destroy(droppable, 30f);
                    } 
                    break;
                    
            }
        }
    }

    // Spawns health at health spawner. Size of health pack depends on wave number
    // Only spawns health on odd waves, (currently) so player gets health before wave 10 (final wave)
    public void SpawnHealth()
    {
        // If world droppables not at max capacity && odd wave
        if (currentDroppables < maxDroppables && wave % 2 > 0)
        {
            if (!healthSpawner.GetComponent<SpawnerOccupied>().occupied)
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
        
    }

    // Register Z has been killed
    public void zDead()
    {
        zLeft-=1;

        zAlive--;
        
        
        // Log Z killed action
        logger.addZKilled();
    }

    // Record the last enemy to do damage to the player
    public void recordLastEnemyToAttack(string tag)
    {
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

    // Changes the possible enemy list depending on wave number
    // Enemies in each list set in unity
    private void ManageEnemyList()
    {
        if (GameObject.FindWithTag("Zevolution").GetComponent<Zevolution>().IsActive() && wave > 1)
        {
            enemyList = GetComponentInChildren<PossibleEnemies>().zevolutionList;
        }
        else
        {
            switch (wave)
            {
                case 1:
                    enemyList = GetComponentInChildren<PossibleEnemies>().tutorialList; // Only easiest Z in list
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
                    enemyList = GetComponentInChildren<PossibleEnemies>().insaneList; // Hardest Zs in list
                    break;
                default:
                    // If max waves > 10
                    enemyList = GetComponentInChildren<PossibleEnemies>().insaneList;
                    break;
            }  
        }
        
    }

    public GameObject getPlayer()
    {
        return player;
    }

    public void SetStress(int s)
    {
        stress = s;
    }

    public int GetStress()
    {
        return stress;
    }

    public float GetSkill()
    {
        return skill;
    }

    public void SetSkill(float s)
    {
        skill = s;
    }

    public void UpdateZAttacking(int Z)
    {
        ZAttacking = Z;
    }

    public int GetZAttacking()
    {
        return ZAttacking;
    }

    public int GetSpawned()
    {
        return spawned;
    }

    public int GetCurrentWaveDR()
    {
        return currentWaveTrueDR;
    }


    public void UpdateZAggroed(int Z)
    {
        ZAggroed = Z;
    }
    
    public int GetZAggroed()
    {
        return ZAggroed;
    }

    public int GetZAlive()
    {
        return zAlive;
    }

    public bool IsInWave()
    {
        return inWave;
    }

    public int GetWave()
    {
        return wave;
    }

    public bool GameWon()
    {
        return win;
    }

    public string GetLastEnemy()
    {
        return lastEnemyToAttack;
    }
}
