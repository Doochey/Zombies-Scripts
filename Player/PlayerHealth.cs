using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    public float health; // Player starting health
    public bool dead; // If player is dead
    public GameObject damageFader; // Red screen overlay, show when player takes damage
    public GameObject poisonFader; // Green screen overlay, Show when player takes poison damage
    

    private Animator anim;
    private Text healthNumber; // Text that displays health number
    private bool poisoned = false; // If player is poisoned
    private StatLogging logger;
    private GameMaster GM;

    void Start()
    {
        healthNumber = GameObject.FindWithTag("health").GetComponent<Text>();
        anim = GetComponent<Animator>();
        
        // Make sure health displays starting health
        updateHealth();
        
        logger = GameObject.FindWithTag("StatLog").GetComponent<StatLogging>();
        GM = GameObject.FindWithTag("GM").GetComponent<GameMaster>();
    }

    void FixedUpdate()
    {
        updateHealth();
        if (health <= 0) // Player is dead
        {
            // Deactivate both overlays
            damageFader.SetActive(false);
            poisonFader.SetActive(false);
            
            dead = true;
            
            // Tell game master game is over
            GM.setGameOver();
            
            // Play death animation
            anim.SetBool("Dying", true);
            
            // Make player invisible
            GameObject.FindWithTag("renderer").GetComponent<Renderer>().enabled = false;
            
            // Stop Player moving and shooting
            Destroy(GetComponent<PlayerMovement>());
            Destroy(GetComponent<PlayerShoot>());
            
            // Destroy player object
            Destroy(gameObject);
        }
    }

    // Reduced player health by damage, tells game master which enemy did damage
    public void takeDamage(float damage, string tag)
    {
        // record enemy that did damage
        GM.recordLastEnemyToAttack(tag);
        
        // Log amount of health lost
        logger.addHealthLost(damage);
        
        // Log times hit
        logger.addTimesHit();
        
        // If health left, So player can not be damaged after death
        if (health > 0)
        {
            // reduce health by damage
            health -= damage;
            
            // Show damage overlay
            StartCoroutine(showDamageFader());
           
            // If damage would have reduced health past 0
            if (health < 0)
            {
                // Set health = 0
                health = 0;
            }

            // Show new health value
            updateHealth();

        }
        
    }

    // Updates the HUD health text
    private void updateHealth()
    {
        healthNumber.text = "" + health; 
    }

    public float getHealth()
    {
        return health;
    }

    // Begins player taking poison damage
    public void takePoison()
    {
        // If not already poisoned
        if (!poisoned)
        { 
            // Begin poison
            poisoned = true;
            
            // Make health text green
            healthNumber.color = new Color(43/255f, 111/255f, 32/255f, 255/255f);
            
            // Log times poisoned
            logger.addTimesPoisoned();
            
            // Start taking poison damage
            StartCoroutine(poisonDamage());
        }
    }
    
    // Reduces player health by 1 every .3 seconds
    private IEnumerator poisonDamage()
    {
        int totalDamage = 0; // Damage done so far
        int damageMax = 10; // Max amount of damge poison can do
        
        while (totalDamage <= damageMax)
        {
            health--;
            
            // Log health lost
            logger.addHealthLost(1);
            
            // Show poison overlay
            StartCoroutine(showPoisonFader());
            
            // Show new health value
            updateHealth();
            
            // Record last 'enemy' to do damage
            GM.recordLastEnemyToAttack("Poisoned");
            
            totalDamage++;
            
            yield return new WaitForSeconds(.3f);
        }
        
        // When max poison damage dealt
        
        // No longer poisoned
        poisoned = false;
        
        // Return health color to white
        healthNumber.color = Color.white;

    }

    // Shows red damage overlay for .1 seconds
    private IEnumerator showDamageFader()
    {
        damageFader.SetActive(true);
        yield return new WaitForSeconds(.1f);
        damageFader.SetActive(false);
    }
    
    // shows green poison overlay for .1 seconds
    private IEnumerator showPoisonFader()
    {
        poisonFader.SetActive(true);
        yield return new WaitForSeconds(.1f);
        poisonFader.SetActive(false);
    }

    // Increases player health by inc amount, used for health pack pickups
    public void increaseHealth(float inc)
    {
        // If health not at max
        if (health < 125)
        {
            // Increase health by inc amount 
            health += inc;
            
            // If inc would have put health past max
            if (health > 125)
            {
                // Set health to max
                health = 125;
            }
        }
        
        // Show new health va;ue
        updateHealth();
    }

    // Stop taking poison damage, used for anti-venom pickup
    public void stopPoison()
    {
        // Not poisoned
        poisoned = false;
        
        // Stop poison damge and screen overlays
        StopAllCoroutines();
        
        // Make sure overlays are deactivated
        poisonFader.SetActive(false);
        damageFader.SetActive(false);
        
        // Reset health color to white
        healthNumber.color = Color.white;
    }

    public bool isPoisoned()
    {
        return poisoned;
    }
}
