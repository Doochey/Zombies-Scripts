using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public float cooldown = 0.3f; // Time that must pass between shots
    public float bulletSpeed = 5f; // Speed bullet moves
    public GameObject projectile; // Prefab for bullet
    public AudioClip shootSound; // Sound made when shooting

    private float currentTime; // Time that has passed since last shot
    int enemyMask; // The layer mask of the enemies
    private AudioSource source; // Audio source attached to player
    private ParticleSystem muzzleFlash; // Particle effect when shooting
    
    void Start()
    {
        currentTime = 100f; // Player should be able to shoot immediately
        enemyMask = LayerMask.GetMask("Enemy");
        source = GetComponent<AudioSource>();
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

    void FixedUpdate()
    {
        // If mouse button clicked
        if (Input.GetMouseButtonDown(1))
        {
            // Log mouse click action on mouse button down
            GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addMouseClicks();
        }
        
        // If mouse button held down && time since last shot >= time between shots
        if (Input.GetMouseButton(1) && currentTime >= cooldown)
        {
            // Raycast through cursor
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit enemyHit;

            // If raycast hit enemy
            if (Physics.Raycast(camRay, out enemyHit, Mathf.Infinity, enemyMask))
            {
                // Bullet should travel towards enemy
                
                // Instatiate bullet
                GameObject bullet = Instantiate(projectile, transform.position, transform.rotation);
                bullet.AddComponent(typeof(Rigidbody));
                Rigidbody bv = bullet.GetComponent<Rigidbody>();
                
                // Disable bullet gravity
                bv.useGravity = false;
                
                // Make bullet face towards destination
                bullet.transform.LookAt(enemyHit.point);
                
                // Set bullet destination
                bullet.GetComponent<BulletMove>().setPoint(enemyHit.point);
                
                // Set bullet speed
                bullet.GetComponent<BulletMove>().setSpeed(bulletSpeed);
                
                // Destroy bullet after time, if it does not hit anything
                Destroy(bullet, 5f);
                
                // Reset time since last shot
                currentTime = 0f;

            }
            else
            {
                // bullet should travel straight forward
                GameObject bullet = Instantiate(projectile, transform.position, transform.rotation);
                bullet.AddComponent(typeof(Rigidbody));
                Destroy(bullet.GetComponent<BulletMove>());
                Rigidbody bv = bullet.GetComponent<Rigidbody>();

                bv.velocity = transform.forward * 50f;
                
                Destroy(bullet, 5f);
                currentTime = 0f;
            }
            
            // Play shooting sound
            source.PlayOneShot(shootSound);
            
            // Play shooting effect
            muzzleFlash.Play();
        }

        // Increase time passed since last shot
        currentTime += Time.deltaTime;
    }
    
}
