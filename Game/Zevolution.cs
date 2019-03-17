using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zevolution : MonoBehaviour
{

    public GameObject player;

    private float stress;
    private float averageStress;
    private float currentWavePeakStress = 0f;
    private float averagePeakStress = 0f;
    private float overallPeakStress = 0f;
    private float skill;

    private int maxStressCount = 0;

    private bool active;
    private bool applyStress;
    private bool cooldownStress;
    private bool madeChanges;
    private bool checkedMaxStress;

    private GameMaster GM;


    private StatLogging logger;

    private void Awake()
    {
        // Change to false to disable Zevolution, Tracking stress/skill still active
        active = false;
    }


    void Start()
    {
        
        
        
        GM = GameObject.FindWithTag("GM").GetComponent<GameMaster>();
        logger = GameObject.FindWithTag("StatLog").GetComponent<StatLogging>();
        averageStress = 0;
        
        // If game restarted get skill from logger, logger is only game object not destroyed on load
        if (logger.GetRestarts() > 0)
        {
            skill = logger.GetSkill();
            
            if (active)
            {
                GM.SetSkill(skill); 
            }
            
        }
        else  // Get skill from GameMaster
        {
            skill = GM.GetSkill(); 
            
            SaveSkill();
        }

        // Zevolution in apply stress state by default
        applyStress = true;
        

        // Start recording average stress
        StartCoroutine(AverageStress());

    }

    void LateUpdate()
    {
        // If Zevolution active, make sure skill is set in GameMaster
        if (active)
        {
            GM.SetSkill(skill);
        }
        
        // Update the amount of stress on the player
        UpdateStress();

        // When wave over, iff applying stress
        if (!GM.IsInWave() && applyStress)
        {
            // Increase the waveDR of the upcoming wave
            IncreaseWaveDR();
            
        }
        
        // When wave over and wave wasn't the first and game is not over
        if (GM.GetWave() != 1 && !GM.IsInWave() && !GM.isGameOver() && !madeChanges)
        {
            Debug.Log("WAVE " + GM.GetWave() + " COMPLETE");
            
            // Update player's skill based on their stress and waveDR
            UpdateSkillWaveComplete();
            
            Debug.Log("New Skill: " + skill);
            
            madeChanges = true;
            
        } else if (GM.isGameOver() && !GM.GameWon() && !madeChanges) // When game over but not won
        {
            Debug.Log("WAVE FAILED");
            
            // Update player's skill based on waveDR
            UpdateSkillWaveFailed();
            
            Debug.Log("New Skill: " + skill);
            
            madeChanges = true;
            
            StopCoroutine(AverageStress());
            
        } else if (GM.isGameOver() && GM.GameWon() && !madeChanges) // When game over and game won
        {
            StopCoroutine(AverageStress());
            
            Debug.Log("GAME COMPLETE");
            
            // Increase players skill by 1
            UpdateSkillGameComplete();
            
            Debug.Log("New Skill: " + skill);
            
            madeChanges = true;
        }

        
    }

    // Increases the waveDRConstant by 5 to increase next waveDR, sets it to 5 if wave was 1
    private void IncreaseWaveDR()
    {
        if (GM.GetWave() == 1)
        {
            GM.waveDRConstant = 5;
        }
        else
        {
            GM.waveDRConstant += 5;
        }
        
    }

    // Reduce the waveDRConstant by 5 to reduce next waveDR
    private void CooldownWaveDR()
    {
        Debug.Log("Cooldown");
        
        GM.waveDRConstant -= 5;
    }
    
    // Reduce the waveDRConstant by 15 to reduce next waveDR dramatically and spawn health if player health low
    private void BigCooldownWaveDR()
    {
        Debug.Log("Big Cooldown");
        
        GM.waveDRConstant -= 15;
        
        if (player.GetComponent<PlayerHealth>().getHealth() < 50)
        {
            GM.SpawnHealth();
        }
        
    }

    // Save the current skill in the logger, logger not destroyed on load
    private void SaveSkill()
    {
        logger.SaveSkill(skill);
    }

    // Updates players stress value 
    private void UpdateStress()
    {
        // Only update if game isn't over
        if (!GM.isGameOver())
        {
            // If wave is in progress
            if (GM.IsInWave())
            {
                madeChanges = false;
                checkedMaxStress = false;
                
                int playerHealth = player.GetComponent<PlayerHealth>().getHealth();
                int healthLost;
                
                // Calculate how much health the player has lost
                if (playerHealth > 100)
                {
                    healthLost = 0;
                }
                else
                {
                    healthLost = 100 - playerHealth;
                }

                // Calculate stress factors
                // Attacker stress - Number of Zs currently attacking the player
                int attackerStress = GM.GetZAttacking() * 20; // Most immediate threat
                
                // Aggro stress - Number of Zs currently chasing the player
                int aggroStress = GM.GetZAggroed() * 2;
                
                // Spawned stress - Number of Zs currently slive in the level
                int spawnedStress = GM.GetZAlive() / 10;
                
                // WaveDRStress - small bonus to stress based on total trueDR of the wave
                float waveDRStress = GM.GetCurrentWaveDR() / 100; // Least immediate threat

                // Calculate total stress
                stress = healthLost + attackerStress + aggroStress + spawnedStress + waveDRStress;

                // Check if new stress value larger than current peak
                CheckPeakStress();
            
            }
            else
            {
                // If wave over, check if stress has reached maximum and add peak to average
                CheckMaxStress();
                AveragePeakStress();
            }
            
        }
        
    }

    // Checks if stress has reached maximum, if so initiates cooldown state
    private void CheckMaxStress()
    {
        // If haven't checked stress already (as called in lateupdate, only want to run once)
        if (!checkedMaxStress && active)
        {
            // If average stress < max && not currently applying stress
            if (averageStress < 100 && !applyStress)
            {
                // Reset max stress
                maxStressCount = 0;
                
                // Switch to applying stress state
                applyStress = true;
                cooldownStress = false;
            }
            else if (averageStress > 100 && maxStressCount == 0) // If average stress > max and not hit max stress yet
            {
                // initiate cooldown state
                cooldownStress = true;
                applyStress = false;
                
                // Add to max stress counter
                maxStressCount++;
                
                // Reduce next waveDR
                CooldownWaveDR();
                    
            } else if (maxStressCount > 0) // If hit max stress before
            {
                // Reduce next waveDR dramatically
                BigCooldownWaveDR();
                
                // Reset max stress count
                maxStressCount = 0;
                    
            }

            checkedMaxStress = true;
        }
    }

    // Checks if current peak stress > old peak
    private void CheckPeakStress()
    {
        if (stress > currentWavePeakStress)
        {
            currentWavePeakStress = stress;
        }

        if (currentWavePeakStress > overallPeakStress)
        {
            overallPeakStress = currentWavePeakStress;
            logger.SetOverallPeakStress(overallPeakStress);
            logger.SetWaveStressPeaked(GM.GetWave());
        }
    }

    // track average of all peak stress
    private void AveragePeakStress()
    {
        if (GM.GetWave() != 1)
        {
            averagePeakStress = averagePeakStress + currentWavePeakStress;
            averagePeakStress = averagePeakStress / 2;
        }
        else
        {
            
            averagePeakStress = currentWavePeakStress;
        }
        
        logger.SetAveragePeakStress(averagePeakStress);
    }


    // Take average stress every 1 second
    private IEnumerator AverageStress()
    {
        while (!GM.isGameOver())
        {
            averageStress = averageStress + stress;
            averageStress = averageStress / 2;
            yield return new WaitForSeconds(1f);
        }

        logger.SetAverageStress(averageStress);
    }

    // Update the player's skill with respect to their stress and waveDR
    private void UpdateSkillWaveComplete()
    {

        // Player gains more skill if their stress < waveDR, lose skill if >
        float skillChange = 1 - averageStress / GM.GetCurrentWaveDR();
        skill += skillChange;
        if (skill < 1)
        {
            skill = 1;
        }

        // Only update GM if Zevolution active
        if (active)
        {
            GM.SetSkill(skill);
        }
        // Save skill in logger
        SaveSkill();
        
    }

    // Updates player's skill depending on the waveDR
    private void UpdateSkillWaveFailed()
    {
        // If waveDR > 100 lose 1 skill level
        if (GM.GetCurrentWaveDR() > 100)
        {
            
            skill--;
        }
        else // Lose skill proportional amount of skill level
        {
            skill -=  1 - GM.GetCurrentWaveDR() / 100;
        }
        
        // Skilll should not be less than 1
        if (skill <= 1)
        {
            skill = 1;
        }
        
        // Only update skill in GM if Zevolution active
        if (active)
        {
            GM.SetSkill(skill);
            
            // Increase DR of enemy that killed player
            GetComponent<ModifiedDR>().AddDR(GM.GetLastEnemy(), 5f);
        }
        
        // Save skill to logger
        SaveSkill();

        
    }
    
    // Increase the player's skill level by 1
    private void UpdateSkillGameComplete()
    {
        skill++;
        if (active)
        {
            GM.SetSkill(skill);
        }
        SaveSkill();
    }

    // Check if Zevolution is active or not
    public bool IsActive()
    {
        return active;
    }

}
