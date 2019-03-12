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
        if (logger.GetRestarts() > 0)
        {
            skill = logger.GetSkill();
            GM.SetSkill(skill);
        }
        else
        {
            skill = GM.GetSkill(); 
            SaveSkill();
        }

        applyStress = true;
        

        StartCoroutine(AverageStress());

    }

    void LateUpdate()
    {
        GM.SetSkill(skill);
        
        UpdateStress();

        if (!GM.IsInWave() && applyStress && active)
        {
            IncreaseWaveDR();
            
        } else if (!GM.IsInWave() && cooldownStress && active)
        {
            CooldownWaveDR();
        }
        
        if (GM.GetWave() != 1 && !GM.IsInWave() && !GM.isGameOver())
        {
            Debug.Log("WAVE COMPLETE");
            UpdateSkillWaveComplete();
        } else if (GM.isGameOver() && !GM.GameWon() && !madeChanges)
        {
            Debug.Log("WAVE FAILED");
            UpdateSkillWaveFailed();
            madeChanges = true;
        } else if (GM.isGameOver() && GM.GameWon() && !madeChanges)
        {
            Debug.Log("GAME COMPLETE");
            UpdateSkillGameComplete();
            madeChanges = true;
        }

        
    }

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

    private void CooldownWaveDR()
    {
        GM.waveDRConstant -= 5;
    }
    
    private void BigCooldownWaveDR()
    {
        GM.waveDRConstant -= 15;
        if (player.GetComponent<PlayerHealth>().getHealth() < 50)
        {
            GM.SpawnHealth();
        }
        
    }

    private void SaveSkill()
    {
        logger.SaveSkill(skill);
    }

    private void UpdateStress()
    {
        if (!GM.isGameOver())
        {
            if (GM.IsInWave())
            {
                int playerHealth = player.GetComponent<PlayerHealth>().getHealth();
                int healthLost;
                if (playerHealth > 100)
                {
                    healthLost = 0;
                }
                else
                {
                    healthLost = 100 - playerHealth;
                }

                int attackerStress = GM.GetZAttacking() * 20;
                int aggroStress = GM.GetZAggroed() * 2;
                int spawnedStress = GM.GetZAlive() / 10;
                int waveDRStress = GM.GetCurrentWaveDR() / 100;

                stress = healthLost + attackerStress + aggroStress + spawnedStress + waveDRStress;

                CheckPeakStress();
            
            }
            else
            {
                if (maxStressCount > 0)
                {
                    BigCooldownWaveDR();
                }
                else if (averageStress > 100 && maxStressCount == 0)
                {
                    
                    cooldownStress = true;
                    applyStress = false;
                    maxStressCount++;
                    
                    Debug.Log("COOLDOWN");
                } else if (averageStress < 100 && !applyStress)
                {
                    applyStress = true;
                    cooldownStress = false;
                    
                    Debug.Log("APPLY STRESS");
                }
                AveragePeakStress();
            }
            
        }
        
    }

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

    private void UpdateSkillWaveComplete()
    {
        if (averageStress < 100)
        {
            skill += (1 - (averageStress / 100));
        }
        else if (averageStress >= 150)
        {
            skill += (1 - (averageStress / 150));
        }

        if (skill <= 1)
        {
            skill = 1;
        }

        if (active)
        {
            GM.SetSkill(skill);
        }
        SaveSkill();
        
    }

    private void UpdateSkillWaveFailed()
    {
        if (GM.GetCurrentWaveDR() > 100)
        {
            
            skill--;
        }
        else
        {
            skill -=  1 - GM.GetCurrentWaveDR() / 100;
        }
        
        if (skill <= 1)
        {
            skill = 1;
        }
        if (active)
        {
            GM.SetSkill(skill);
        }
        SaveSkill();


        if (active)
        {
            GetComponent<ModifiedDR>().AddDR(GM.GetLastEnemy(), 5f);
        }
        
    }
    
    private void UpdateSkillGameComplete()
    {
        skill++;
        if (active)
        {
            GM.SetSkill(skill);
        }
        SaveSkill();
    }

    public bool IsActive()
    {
        return active;
    }

}
