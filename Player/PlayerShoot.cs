using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public float cooldown = 0.3f;
    public float bulletSpeed = 5f;
    public GameObject projectile;
    public AudioClip shootSound;

    private float currentTime;
    float camRayLength = 100f; 
    int enemyMask;
    private AudioSource source;
    private ParticleSystem muzzleFlash;
    
    void Start()
    {
        currentTime = 100f;
        enemyMask = LayerMask.GetMask("Enemy");
        source = GetComponent<AudioSource>();
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addMouseClicks();
        }
        if (Input.GetMouseButton(1) && currentTime >= cooldown)
        {
            
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit enemyHit;

            if (Physics.Raycast(camRay, out enemyHit, camRayLength, enemyMask))
            {
                
                GameObject bullet = Instantiate(projectile, transform.position, transform.rotation);
                bullet.AddComponent(typeof(Rigidbody));
                Rigidbody bv = bullet.GetComponent<Rigidbody>();
                
                bv.useGravity = false;
                bullet.transform.LookAt(enemyHit.point);
                bullet.GetComponent<BulletMove>().setPoint(enemyHit.point);
                bullet.GetComponent<BulletMove>().setSpeed(bulletSpeed);
                
                
                Destroy(bullet, 5f);
                currentTime = 0f;

            }
            else
            {
                GameObject bullet = Instantiate(projectile, transform.position, transform.rotation);
                bullet.AddComponent(typeof(Rigidbody));
                Destroy(bullet.GetComponent<BulletMove>());
                Rigidbody bv = bullet.GetComponent<Rigidbody>();

                bv.velocity = transform.forward * 50f;
                
                Destroy(bullet, 5f);
                currentTime = 0f;
            }
            source.PlayOneShot(shootSound);
            muzzleFlash.Play();
        }

        currentTime += Time.deltaTime;
    }
    
}
