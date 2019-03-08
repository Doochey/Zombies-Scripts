using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    public float health;
    public bool dead;
    public GameObject damageFader;
    public GameObject poisonFader;
    

    private Animator anim;
    private Text healthNumber;
    private bool poisoned = false;

    void Start()
    {
        healthNumber = GameObject.FindWithTag("health").GetComponent<Text>();
        anim = GetComponent<Animator>();
        updateHealth();
    }

    void FixedUpdate()
    {
        updateHealth();
        if (health <= 0)
        {
            damageFader.SetActive(false);
            poisonFader.SetActive(false);
            dead = true;
            GameObject.FindWithTag("GM").GetComponent<GameMaster>().setGameOver();
            // Play death animation
            anim.SetBool("Dying", true);
            
            // Stop Player moving and shooting
            GameObject.FindWithTag("renderer").GetComponent<Renderer>().enabled = false;
            Destroy(GetComponent<PlayerMovement>());
            Destroy(GetComponent<PlayerShoot>());
            Destroy(gameObject);
        }
    }

    public void takeDamage(float damage, string tag)
    {
        GameObject.FindWithTag("GM").GetComponent<GameMaster>().recordLastEnemyToAttack(tag);
        GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addHealthLost(damage);
        GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addTimesHit();
        if (health > 0)
        {
            health -= damage;
            StartCoroutine(showDamageFader());
           
            if (health < 0)
            {
                health = 0;
            }

            updateHealth();

        }
        
    }

    private void updateHealth()
    {
        healthNumber.text = "" + health; 
    }

    public float getHealth()
    {
        return health;
    }

    public void takePoison()
    {
        if (!poisoned)
        {
            poisoned = true;
            healthNumber.color = new Color(43/255f, 111/255f, 32/255f, 255/255f);
            GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addTimesPoisoned();
            StartCoroutine(poisonDamage());
        }
    }
    private IEnumerator poisonDamage()
    {
        int totalDamage = 0;
        int damageMax = 10;
        while (totalDamage <= damageMax)
        {
            health--;
            GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addHealthLost(1);
            StartCoroutine(showPoisonFader());
            updateHealth();
            GameObject.FindWithTag("GM").GetComponent<GameMaster>().recordLastEnemyToAttack("Poisoned");
            totalDamage++;
            yield return new WaitForSeconds(.3f);
        }
        

        poisoned = false;
        healthNumber.color = Color.white;

    }

    private IEnumerator showDamageFader()
    {
        damageFader.SetActive(true);
        yield return new WaitForSeconds(.1f);
        damageFader.SetActive(false);
    }
    
    private IEnumerator showPoisonFader()
    {
        poisonFader.SetActive(true);
        yield return new WaitForSeconds(.1f);
        poisonFader.SetActive(false);
    }

    public void increaseHealth(float inc)
    {
        if (health < 125)
        {
            health += inc;
            if (health > 125)
            {
                health = 125;
            }
        }
        
        updateHealth();
    }

    public void stopPoison()
    {
        poisoned = false;
        StopAllCoroutines();
        poisonFader.SetActive(false);
        damageFader.SetActive(false);
        healthNumber.color = Color.white;
    }

    public bool isPoisoned()
    {
        return poisoned;
    }
}
