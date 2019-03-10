using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zevolution : MonoBehaviour
{

    private float stress;
    private float averageStress;
    private float currentWavePeakStress = 0f;
    private float averagePeakStress = 0f;
    private float overalPeakStress = 0f;
    private float skill;

    private bool active;

    private GameMaster GM;

    private GameObject player;

    private StatLogging logger;

    private static bool created = false;
    
    private bool stopped = false;
    private bool restarted = false;
    
    void Start()
    {
        if (created)
        {
            Destroy(this.gameObject);
        }
        created = true;
        
        // Comment out line below to disable Zevolution, Tracking still active
        active = true;
        
        GM = GameObject.FindWithTag("GM").GetComponent<GameMaster>();
        player = GameObject.FindWithTag("Player");
        logger = GameObject.FindWithTag("StatLog").GetComponent<StatLogging>();
        skill = GM.GetSkill();

        StartCoroutine(AverageStress());

    }

    void LateUpdate()
    {
        if (restarted)
        {
            
            GM = GameObject.FindWithTag("GM").GetComponent<GameMaster>();
            player = GameObject.FindWithTag("Player");
            GM.SetSkill(skill);
            restarted = false;
            StartCoroutine(AverageStress());
        }
        
        UpdateStress();

        if (active)
        {
            if (GM.GetWave() != 1 && !GM.IsInWave() && !GM.isGameOver())
            {
                UpdateSkillWaveComplete();
            } else if (GM.isGameOver() && !GM.GameWon())
            {
                UpdateSkillWaveFailed();
            } else if (GM.isGameOver() && GM.GameWon())
            {
                UpdateSkillGameComplete();
            }
        }
        
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
            
                Debug.Log("Stress: " + stress);
                Debug.Log("Skill: " + skill);
            }
            else
            {
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

        if (currentWavePeakStress > overalPeakStress)
        {
            overalPeakStress = currentWavePeakStress;
            logger.SetOverallPeakStress(overalPeakStress);
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
        GM.SetSkill(skill);
        
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
        GM.SetSkill(skill);
    }
    
    private void UpdateSkillGameComplete()
    {
        skill++;
        GM.SetSkill(skill);
    }

    public void Restarted()
    {
        restarted = true;
    }
}
