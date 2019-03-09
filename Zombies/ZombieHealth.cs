using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public float health = 5f; // Z Starting health
    
    public GameObject littleZ = null; // Prefab for minion spawning, Only arachnid
    
    private Animator anim;
    private bool dying = false; // If Z is dying
    private bool spawning; // If Z is spwning minions
    private bool regen = false; // If Z is regenerating health, mutant only
    
    private float maxHealth; // Max health Z can have, for regenerating
    private float regenCooldown = 3f; // Time between Z hit and can regenerate health
    private float currentTime; // Time passed since hit
    
    private AudioSource source;
    
    private AudioClip[] hurtSounds; // Sounds for Z getting hit
    private AudioClip[] impactSounds; // Sounds for bullet hitting Z
    
    private GameMaster GM;
    
    
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        maxHealth = health;
        source = gameObject.GetComponent<AudioSource>();
        hurtSounds = GameObject.FindWithTag("Sounds").GetComponent<ZombieSounds>().hurtSounds;
        impactSounds = GameObject.FindWithTag("Sounds").GetComponent<ZombieSounds>().impactSounds;
        GM = GameObject.FindWithTag("GM").GetComponent<GameMaster>();
    }

    void FixedUpdate()
    {
        // When Z health is 0
        if (health <= 0)
        {
            if (!dying) // NOT dying as should only run first time health is registered <= 0
            {
                // Register Z dead with Game Master
                GM.zDead(); 
                
                // Chance of dropping item
                GM.drop(gameObject);
                
            }
            
            // Now set dying
            dying = true;
            
            // Set anim states
            anim.SetBool("dying", true);
            anim.SetBool("walking", false);
            anim.SetBool("idle", false);
            anim.SetBool("attacking", false);
            
            // Stop Zombie from moving
            Destroy(gameObject.GetComponent<ZombieAI>());
            
            // Stop Z from colliding with other Z, player and bullets
            Destroy(gameObject.GetComponent<BoxCollider>());
            
            // Freeze X,Y,Z positions and rotations
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            
            // Change layer so bullet raycast does not register
            gameObject.layer = 0;

            // If arachnid
            if (gameObject.tag.Equals("Arachnid"))
            {
                // If all minions spawned
                if (GetComponent<SpawnMinions>().doneSpawning)
                {
                    // Destroy Z
                    Destroy(gameObject);
                }
                else
                {
                    // If not spawning minions
                    if (!spawning)
                    {
                        // Start spawning minions
                        StartCoroutine(GetComponent<SpawnMinions>().spawnMinion());
                        
                        // Now spawning
                        spawning = true;
                    }
                    
                }
            }
            else
            {
                // Destroy Z
                Destroy(gameObject, 2f);  
            }
            
        } else if (gameObject.tag.Equals("Mutant")) // If health !<= 0 && Z is a mutant
        {
            // If its not currently regenerating && Time since hit > cooldown
            if (!regen && currentTime >= regenCooldown)
            {
                // Start regenerating
                StartCoroutine(regenHealth());
            }
            else
            {
                // Increase time since hit
                currentTime += Time.deltaTime;
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && !dying) // If bullet hits Z and Z is not dying
        {
            // Play sounds of hit
            source.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
            source.PlayOneShot(impactSounds[Random.Range(0, impactSounds.Length)]);
            
            // Play blood spatter effect
            GetComponentInChildren<ParticleSystem>().Play();
            
            // If regenerating, Mutant only
            if (regen)
            {
                // Time since hit = 0
                currentTime = 0;
                
                // Stop any regenerating currently going
                StopCoroutine(regenHealth());
                regen = false;
            }
            
            // REduce health by 1
            health--;
            
            // Destroy bullet
            Destroy(other.gameObject);
            
        }
    }

    // Increases health by 1 every 2 seconds
    private IEnumerator regenHealth()
    {
        regen = true;
        while (health != maxHealth)
        {
            health++;
            yield return new WaitForSeconds(2f);
        }
    }

    public float getHealth()
    {
        return health;
    }
}
