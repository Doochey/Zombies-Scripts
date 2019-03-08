using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject player;
    
    private bool paused;
    
    
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !paused)
        {
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && paused)
        {
            UnpauseGame();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        paused = true;
        
        GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addTimesPaused();

        if (!GameObject.FindWithTag("GM").GetComponent<GameMaster>().isGameOver())
        {
            player.GetComponent<PlayerMovement>().enabled = false;
            player.GetComponentInChildren<PlayerShoot>().enabled = false;
        }
        
        
        pauseMenu.SetActive(true);
    }
    
    public void UnpauseGame()
    {
        if (!GameObject.FindWithTag("GM").GetComponent<GameMaster>().isGameOver())
        {
            player.GetComponent<PlayerMovement>().enabled = true;
            player.GetComponentInChildren<PlayerShoot>().enabled = true;
        }
        
        
        
        Time.timeScale = 1f;
        paused = false;
        
        pauseMenu.SetActive(false);

        
    }

    public void RestartGame()
    {
        GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addTimesRestarted();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
      
        GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().StatDump();
        Application.Quit();
        
    }

    public void ShowMenu()
    {
        pauseMenu.SetActive(true);
    }

    public void HideMenu()
    {
        pauseMenu.SetActive(false);
    }
}
