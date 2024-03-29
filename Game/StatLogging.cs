﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatLogging : MonoBehaviour
{
    

    private float timePlayed = 0f; // Time game has been running
    private float averageWaveDR = 0f; // Average 'trueDR' of waves player faced
    private float healthLost = 0f; // Total health player lost
    private float trueDRWaveStressPeaked = 0;
    private float averageStress = 0f;
    private float averageWavePeakStress = 0f;
    private float overallPeakStress = 0f;

    private string killedPlayer = ""; //  Which enemy killed the player

    private int zKilled = 0; // Number of Zs player killed
    private int waveReached = 1; // The max wave player reached
    private int healthPacksPickedUp = 0; // Number of health packs picked up
    private int timesHit = 0; // Number of times player hit
    private int timesPaused = 0; // Number of times player paused the game
    private int timesRestarted = 0; // Number of times the player restarted the game
    private int mouseClicks = 0; // Number of times player clicked button on mouse
    private int AVPickedUp = 0; // Number of Anti venom picked up
    private int timesPoisoned = 0; // Number of times player was poisoned
    private int gamesWon = 0; // Number of games the player has won
    private int waveStressPeaked = 1;
    private float skill = 0;

    private static bool created = false;
    private bool dumped = false; // If stats have been printed to output


    private void Start()
    {
        if (created)
        {
            Destroy(this.gameObject);
        }
        else
        {
            ClearFile();
        }

        created = true;
    }

    public void SaveSkill(float s)
    {
        skill = s;
    }

    public float GetSkill()
    {
        return skill;
    }

    public int GetRestarts()
    {
        return timesRestarted;
    }

    private float getTimePlayed()
    {
        timePlayed = Time.time;
        return timePlayed;
    }

    public void SetKilledPlayer(string enemy)
    {
        killedPlayer = enemy;
    }

    public void addZKilled()
    {
        zKilled++;
    }
    
    public void addWaveReached(int wave)
    {
        waveReached = wave;
    }
    public void addHealthLost(float hl)
    {
        healthLost += hl;
    }
    public void addHealthPacksPickedUp()
    {
        healthPacksPickedUp++;
    }
    public void addTimesHit()
    {
        timesHit++;
    }
    public void addTimesPaused()
    {
        timesPaused++;
    }
    public void addTimesRestarted()
    {
        timesRestarted++;
    }
    public void addMouseClicks()
    {
        mouseClicks++;
    }
    public void addAV()
    {
        AVPickedUp++;
    }
    public void addTimesPoisoned()
    {
        timesPoisoned++;
    }
    public void addWaveDR(float waveDR)
    {
        averageWaveDR += waveDR;
        averageWaveDR = averageWaveDR / 2;
    }

    public void addGamesWon()
    {
        gamesWon++;
    }

    public void SetAverageStress(float average)
    {
        averageStress = average;
    }

    public void SetAveragePeakStress(float average)
    {
        averageWavePeakStress = average;
    }

    public void SetOverallPeakStress(float stress)
    {
        overallPeakStress = stress;
    }

    public void SetWaveStressPeaked(int wave)
    {
        waveStressPeaked = wave;
    }

    public void StatDump()
    {
        // Print the stats to output
        // Currently debug file /users/appdata/locallow/companyname/productname/output.txt
        if (!dumped)
        {
            Debug.Log("Zombies Killed: " + zKilled);
            Debug.Log("Time Played: " + getTimePlayed());
            Debug.Log("Killed player:  " + killedPlayer);
            Debug.Log("Wave Reached: " + waveReached);
            Debug.Log("Health Lost: " + healthLost);
            Debug.Log("Health Packs Picked Up: " + healthPacksPickedUp);
            Debug.Log("Times hit: " + timesHit);
            Debug.Log("Times Paused: " + timesPaused);
            Debug.Log("Times Restarted: " + timesRestarted);
            Debug.Log("Mouse Clicks: " + mouseClicks);
            Debug.Log("Anti Venom Picked up: " + AVPickedUp);
            Debug.Log("Times Poisoned: " + timesPoisoned);
            Debug.Log("Average Wave DR: " + averageWaveDR);
            Debug.Log("Games Won: " + gamesWon);

            
            System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\Users\Public\Unity Stat Dumps\stat_dump.txt", true);
            file.WriteLine("Start Dump --------------------- ");
            file.WriteLine("Zevolution Status: " + GameObject.FindWithTag("Zevolution").GetComponent<Zevolution>().IsActive());
            file.WriteLine("Time Played: " + getTimePlayed());
            file.WriteLine("Wave Reached: " + waveReached);
            file.WriteLine("Average Wave DR: " + averageWaveDR);
            file.WriteLine("Killed player:  " + killedPlayer);
            file.WriteLine("Health Lost: " + healthLost);
            file.WriteLine("Health Packs Picked Up: " + healthPacksPickedUp);
            file.WriteLine("Times hit: " + timesHit);
            file.WriteLine("Average Stress: " + averageStress);
            file.WriteLine("Overall Peak Stress: " + overallPeakStress);
            file.WriteLine("Average Peak Stress: " + averageWavePeakStress);
            file.WriteLine("Wave Stress Peaked: " + waveStressPeaked);
            file.WriteLine("Skill: " + skill);
            file.WriteLine("Zombies Killed: " + zKilled);
            file.WriteLine("Times Paused: " + timesPaused);
            file.WriteLine("Times Restarted: " + timesRestarted);
            file.WriteLine("Mouse Clicks: " + mouseClicks);
            file.WriteLine("Anti Venom Picked up: " + AVPickedUp);
            file.WriteLine("Times Poisoned: " + timesPoisoned);
            file.WriteLine("Games Won: " + gamesWon);
            
            
            file.WriteLine("End Dump --------------------- ");
            file.Close();
            
            dumped = true;
        }
        
        
    }

    public void resetDumped()
    {
        dumped = false;
    }

    private void ClearFile()
    {
        System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\Public\Unity Stat Dumps\stat_dump.txt");
        file.Close();
    }

    public void resetStats()
    {
        waveReached = 0;
        averageWaveDR = 0;
        killedPlayer = "";
        healthLost = 0;
        healthPacksPickedUp = 0;
        timesHit = 0;
        averageStress = 0;
        overallPeakStress = 0;
        averageWavePeakStress = 0;
        waveStressPeaked = 0;
        zKilled = 0;
        AVPickedUp = 0;
        timesPoisoned = 0;

    }
}
