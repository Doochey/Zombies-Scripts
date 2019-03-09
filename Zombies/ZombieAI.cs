using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    
    
    public float attackRange; // Maximum distance between Z and Player that Z can attack from
    public float speed; // Speed Z can move
    public float timeToDamage; // Minimum time Z must be in range before an attack is made
    public float damage; // Damage Z can do to player
    public float DR; // Difficulty rating of Z
    public float noiseRate; // How often Z can make sound
    public float maxRange; // Max aggro range of Z


    // Insect only v
    public GameObject poisonBall; // Prefab of poison ball
    
    public float spitSpeed; // Speed poison moves
    // Insect only ^

    
    private Animator anim;
    
    private GameObject player;
    private GameObject GM; // GameMaster
    
    private AudioSource source;
    
    private float noiseCooldown; // Time that must pass before Z can make noise
    private float timeInRange = 0f; // Length of time Z is in range
    
    private AudioClip[] groans; // list of groaning sounds
    private AudioClip[] attackSounds; // List of attack sounds

    private bool inLOS = false; // If player is in line of sight (LOS)
    private bool attackAdded = false; // Has Z been added to total attacking pool
    private bool soundAdded = false; // Has Z been added to total sound pool
    

    private static int soundsPlaying = 0; // Current number of sounds playing
    private static int maxSounds = 3; // Maximum number of sounds allowed to play
    private static int numberZAttacking = 0; // Current number of Z within attacking range of player
    
    
    
    
    
    
    void Start()
    {
        player = GameObject.FindWithTag("GM").GetComponent<GameMaster>().getPlayer();
        GM = GameObject.FindWithTag("GM");
        anim = gameObject.GetComponent<Animator>();
        source = gameObject.GetComponent<AudioSource>();
        groans = GameObject.FindWithTag("Sounds").GetComponent<ZombieSounds>().groans;
        attackSounds = GameObject.FindWithTag("Sounds").GetComponent<ZombieSounds>().attackSounds;
        
        // Random offset for noise rate, prevents all Z making noises at same rate
        noiseRate += Random.Range(1, 8f);
    }

    void FixedUpdate()
    {
        // If game is not over and Z is alive ( Is an undead alive? )
        if (!GM.GetComponent<GameMaster>().isGameOver() && GetComponent<ZombieHealth>().health != 0f)
        {

            // Chance to make noise from possible noises
            makeNoise(groans[Random.Range(0, groans.Length)]);
            
            // Raycast line of sight
            RaycastHit los;
            
            // Origin of ray just above Z transform, Z transform on floor
            Vector3 origin = transform.position;
            origin.y = 2f;
            
            // Ray destination just above player transform
            Vector3 end = player.transform.position;
            end.y = 2f;
            var rayDirection = end - origin;
            
            LayerMask layerMask = LayerMask.GetMask("Default"); // So raycast ignores spawn points and other enemies
            
            // If ray hits anything
            if (Physics.Raycast(origin, rayDirection, out los, Mathf.Infinity, layerMask))
            {
                // If ray hits player
                if (los.transform == player.transform)
                {
                    // Player is in LOS
                    inLOS = true;
                }
                else
                {
                    // Player is not in LOS
                    inLOS = false;
                }
            }
            
            
  
            // If distance between Z and player > max aggro range or player not in LOS
            if (Vector3.Distance(transform.position, player.transform.position) >= maxRange || !inLOS)
            {
                // Play idle animation
                anim.SetBool("idle", true);
                anim.SetBool("attacking", false);
                anim.SetBool("walking", false);

                // Reset time in range
                timeInRange = 0f;

                
            } 
            // if not in attack range && player in LOS, move towards player
            else if (Vector3.Distance(transform.position, player.transform.position) >= attackRange && inLOS)
            {
                if (attackAdded) // If Z was previously in attack range
                {
                    // Remove them from total attacking pool
                    attackAdded = false;
                    numberZAttacking--;
                }
                
                // Reptile only
                if (gameObject.tag.Equals("Reptile"))
                {
                    // Find 'beacon' (Gameobject in fron of player)
                    GameObject beacon = GameObject.FindWithTag("Beacon");
                    
                    // Face beacon
                    transform.LookAt(beacon.transform);
                    
                    // Minimum range between rpetile and beacon
                    float beaconRange = 5f;

                    if (Vector3.Distance(transform.position, beacon.transform.position) >= beaconRange)
                    {
                        // Move forward toward beacon
                        transform.position += transform.forward * speed * Time.deltaTime;
                    }
                    else
                    {
                        // Face player
                        transform.LookAt(player.transform);
                
                
                        // Move forward
                        transform.position += transform.forward * speed * Time.deltaTime;
                    }

                    
                }
                else
                {
                    // Face player
                    transform.LookAt(player.transform);
                
                
                    // Move forward
                    transform.position += transform.forward * speed * Time.deltaTime;

                }

                timeInRange = 0f;
                
                // Play walking animation
                anim.SetBool("walking", true);
                anim.SetBool("idle", false);
                anim.SetBool("attacking", false);
            }
            else if (inLOS)// Player must be within attack range now. Attack
            {
                // Face player
                transform.LookAt(player.transform);
                
                // Add Z to total attacking pool
                attackAdded = true;
                numberZAttacking++;
                
                // Increase Time Z is in attack range (Z should not do damage as soon as in range)
                timeInRange += Time.deltaTime;
                
                // Begin attacking animation
                anim.SetBool("attacking", true);
                anim.SetBool("walking", false);
                anim.SetBool("idle", false);
                
                if (timeInRange >= timeToDamage) // If Z was in range for time limit
                {
                    noiseCooldown = 100; // Z Should always make sound when attacking
                    
                    // Make attacking noise
                    makeNoise(attackSounds[Random.Range(0, attackSounds.Length)]);  
                    
                    // Insect only
                    if (gameObject.tag.Equals("Insect"))
                    {
                        // Instantiate poison ball
                        GameObject spit = Instantiate(poisonBall, gameObject.transform.GetChild(0).transform.position, transform.rotation);
                        spit.GetComponent<PoisionBallCollision>().setDamage(damage);
                        
                        // Face poison ball towards player
                        spit.transform.LookAt(player.transform);
                        Rigidbody spitRB = spit.GetComponent<Rigidbody>();
                        
                        // Disable poison bal lgravity
                        spitRB.useGravity = false;
                        
                        // Move poison ball forward
                        spitRB.velocity = transform.forward * spitSpeed;
                        
                        // De spawn spit after 10 seconds
                        Destroy(spit, 10f);

                        timeInRange = 0f;
                    }
                    else
                    {
                        
                        // Player takes damage
                        player.GetComponent<PlayerHealth>().takeDamage(damage, gameObject.tag);
                    
                        // Reset range timer
                        timeInRange = 0f;
                    }
                   
                    
                }
            } 
        }
        else // Game must be over or Z dead
        {
            // Play idle animation
            anim.SetBool("idle", true);
            anim.SetBool("attacking", false);
            anim.SetBool("walking", false);
        }
        
        
    }

    // Chance of Z making noise if time since last noise > cooldown && number of noises playing not at max
    void makeNoise(AudioClip clip)
    {
        
        noiseCooldown += Time.deltaTime;
        if (Random.Range(0, 10) != 0 && noiseCooldown > noiseRate && soundsPlaying < maxSounds)
        {
            source.PlayOneShot(clip);
            noiseCooldown = 0f;
            soundsPlaying++;
            soundAdded = true;
        }

        if (!source.isPlaying && soundAdded)
        {
            soundsPlaying--;
        }

    }

      
    }
    


