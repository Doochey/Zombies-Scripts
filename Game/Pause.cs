using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu; //Menu to display when user pauses game
    public GameObject player;
    
    private bool paused; // If the game is currently paused
    private StatLogging logger;
    private GameMaster GM;
    
    
    void Start()
    {
        logger = GameObject.FindWithTag("StatLog").GetComponent<StatLogging>();
        GM = GameObject.FindWithTag("GM").GetComponent<GameMaster>();
    }

    void Update()
    {
        // Space to pause/unpause
        if (Input.GetKeyDown(KeyCode.Space) && !paused)
        {
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && paused)
        {
            UnpauseGame();
        }
        
        // Escape to quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }

        // R to restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    // Pauses the game
    public void PauseGame()
    {
        if (!GM.isGameOver())
        {
            // Stop time
            Time.timeScale = 0f;
            paused = true;
        
            // Log pause action
            logger.addTimesPaused();

            player.GetComponent<PlayerMovement>().enabled = false;
            player.GetComponentInChildren<PlayerShoot>().enabled = false;
            
            pauseMenu.SetActive(true);
        }
    }
    
    // Unpauses the game
    public void UnpauseGame()
    {
        if (!GM.isGameOver())
        {
            player.GetComponent<PlayerMovement>().enabled = true;
            player.GetComponentInChildren<PlayerShoot>().enabled = true;
            
            Time.timeScale = 1f;
            paused = false;
        
            pauseMenu.SetActive(false);
        }
    }

    // Reloads the game
    public void RestartGame()
    {
        // Log restart action
        logger.addTimesRestarted();
        
        // Ensure time running
        Time.timeScale = 1f;
        
        logger.resetDumped();
        logger.StatDump();
        
        // Load same scene
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        // Print Log to output
        logger.resetDumped();
        logger.StatDump();
        Application.Quit();
        
    }

    // Displays pause menu, for use when switching between zombpedia and pause menu
    public void ShowMenu()
    {
        pauseMenu.SetActive(true);
    }

    // Hides pause menu, for use when switching between zombpedia and pause menu
    public void HideMenu()
    {
        pauseMenu.SetActive(false);
    }
}
