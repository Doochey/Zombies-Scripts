﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    
    
    public float attackRange; // Maximum distance between Z and Player that Z can attack from
    public float speed;
    public float timeToDamage; // Minimum time Z must be in range before an attack is made
    public float damage;
    public float DR; // Difficulty rating of Z
    public float noiseRate; // How often Z can make sound
    public float maxRange;


    public GameObject poisonBall;
    public float spitSpeed;


    private float noiseCooldown;
    private Animator anim;
    private GameObject player;
    private AudioSource source;
    private float timeInRange = 0f; // Length of time Z is in range
    private GameObject GM; // GameMaster
    private AudioClip[] groans;
    private AudioClip[] attackSounds;

    private bool inLOS = false;
    

    private static int soundsPlaying = 0; // Current number of sounds playing
    private static int maxSounds = 3; // Maximum number of sounds allowed to play
    private static int numberZAttacking = 0; // Current number of Z within attacking range of player
    private bool attackAdded = false; // Has Z been added to total attacking pool
    private bool soundAdded = false; // Has Z been added to total sound pool
    
    
    
    
    void Start()
    {
        player = GameObject.FindWithTag("GM").GetComponent<GameMaster>().getPlayer();
        GM = GameObject.FindWithTag("GM");
        anim = gameObject.GetComponent<Animator>();
        source = gameObject.GetComponent<AudioSource>();
        groans = GameObject.FindWithTag("Sounds").GetComponent<ZombieSounds>().groans;
        attackSounds = GameObject.FindWithTag("Sounds").GetComponent<ZombieSounds>().attackSounds;
        noiseRate += Random.Range(1, 6f);
    }

    void FixedUpdate()
    {
        // If game is not over and Z is alive ( Is an undead alive? )
        if (!GM.GetComponent<GameMaster>().isGameOver() && GetComponent<ZombieHealth>().health != 0f)
        {

            
            makeNoise(groans[Random.Range(0, groans.Length)]);
            
            // Raycast line of sight
            RaycastHit los;
            Vector3 origin = transform.position;
            origin.y = 2f;
            Vector3 end = player.transform.position;
            end.y = 2f;
            var rayDirection = end - origin;
            LayerMask layerMask = LayerMask.GetMask("Default"); // So raycast ignores spawn points and other enemies
            if (Physics.Raycast(origin, rayDirection, out los, Mathf.Infinity, layerMask))
            {
                if (los.transform == player.transform)
                {
                    inLOS = true;
                }
                else
                {
                    inLOS = false;
                }
            }
            
            
  
            if (Vector3.Distance(transform.position, player.transform.position) >= maxRange || !inLOS)
            {
                // Play idle animation
                anim.SetBool("idle", true);
                anim.SetBool("attacking", false);
                anim.SetBool("walking", false);

                timeInRange = 0f;

                
            } 
            // if not in attack range move towards player
            else if (Vector3.Distance(transform.position, player.transform.position) >= attackRange && inLOS)
            {
                if (attackAdded) // If Z was previously in attack range
                {
                    // Remove them from total attacking pool
                    attackAdded = false;
                    numberZAttacking--;
                }
                
                if (gameObject.tag.Equals("Reptile"))
                {
                    GameObject beacon = GameObject.FindWithTag("Beacon");
                    transform.LookAt(beacon.transform);
                    float beaconRange = 5f;

                    if (Vector3.Distance(transform.position, beacon.transform.position) >= beaconRange)
                    {
                        // Move forward
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
            else if (inLOS)// Attack
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
                    
                    makeNoise(attackSounds[Random.Range(0, attackSounds.Length)]);  
                    
                    if (gameObject.tag.Equals("Insect"))
                    {
                        
                        GameObject spit = Instantiate(poisonBall, gameObject.transform.GetChild(0).transform.position, transform.rotation);
                        spit.GetComponent<PoisionBallCollision>().setDamage(damage);
                        spit.transform.LookAt(player.transform);
                        Rigidbody spitRB = spit.GetComponent<Rigidbody>();
                        spitRB.useGravity = false;
                        spitRB.velocity = transform.forward * spitSpeed;
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
        else
        {
            // Play idle animation
            anim.SetBool("idle", true);
            anim.SetBool("attacking", false);
            anim.SetBool("walking", false);
        }
        
        
    }

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
    

