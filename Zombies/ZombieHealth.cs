using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public float health = 5f;
    public GameObject littleZ = null;
    
    private Animator anim;
    private bool dying = false;
    private bool spawning;
    private float maxHealth;
    private bool regen = false;
    private float regenCooldown = 3f;
    private float currentTime;
    private AudioSource source;
    private AudioClip[] hurtSounds;
    private AudioClip[] impactSounds;
    
    
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        maxHealth = health;
        source = gameObject.GetComponent<AudioSource>();
        hurtSounds = GameObject.FindWithTag("Sounds").GetComponent<ZombieSounds>().hurtSounds;
        impactSounds = GameObject.FindWithTag("Sounds").GetComponent<ZombieSounds>().impactSounds;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (health <= 0)
        {
            if (!dying) // NOT dying as should only run first time health is registered <= 0
            {
                GameObject.FindWithTag("GM").GetComponent<GameMaster>().zDead(); 
                GameObject.FindWithTag("GM").GetComponent<GameMaster>().drop(gameObject);
                
            }
            dying = true;
            
            // Set anim states
            anim.SetBool("dying", true);
            anim.SetBool("walking", false);
            anim.SetBool("idle", false);
            anim.SetBool("attacking", false);
            
            // Stop Zombie from moving
            Destroy(gameObject.GetComponent<ZombieAI>());
            
            Destroy(gameObject.GetComponent<BoxCollider>());
            
            // Freeze X,Y,Z positions and rotations
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            
            // Change layer so bullet raycast does not register
            gameObject.layer = 0;

            if (gameObject.tag.Equals("Arachnid"))
            {
                if (GetComponent<SpawnMinions>().doneSpawning)
                {
                    Destroy(gameObject);
                }
                else
                {
                    if (!spawning)
                    {
                        StartCoroutine(GetComponent<SpawnMinions>().spawnMinion());
                        spawning = true;
                    }
                    
                }
            }
            else
            {
                Destroy(gameObject, 2f);  
            }
            
        } else if (gameObject.tag.Equals("Mutant"))
        {
            if (!regen && currentTime >= regenCooldown)
            {
                StartCoroutine(regenHealth());
            }
            else
            {
                currentTime += Time.deltaTime;
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && !dying)
        {
            source.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
            source.PlayOneShot(impactSounds[Random.Range(0, impactSounds.Length)]);
            GetComponentInChildren<ParticleSystem>().Play();
            if (regen)
            {
                currentTime = 0;
                StopCoroutine(regenHealth());
                regen = false;
            }
            
            health--;
            Destroy(other.gameObject);
            
        }
    }

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
