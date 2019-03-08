using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatLogging : MonoBehaviour
{
    private int zKilled = 0;

    private float timePlayed = 0f;

    private string killedPlayer = "";

    private int waveReached = 1;
    private float healthLost = 0f;
    private int healthPacksPickedUp = 0;
    private int timesHit = 0;
    private int timesPaused = 0;
    private int timesRestarted = 0;
    private int mouseClicks = 0;
    private int AVPickedUp = 0;
    private int timesPoisoned = 0;
    private float averageWaveDR = 0f;
    private bool dumped = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void StatDump()
    {
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
            dumped = true;
        }
        
        
    }

    public void resetStats()
    {
        
    }
}
