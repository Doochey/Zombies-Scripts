using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;       
    Vector3 movement;                    
    Rigidbody player;           
    private Animator anim;
    private bool moving;
    float camRayLength = 100f; 
    int floorMask; 

    void Start()
    {
        player = GetComponent <Rigidbody> ();
        anim = GetComponent<Animator>();
        
        floorMask = LayerMask.GetMask("Floor");
    }


    void FixedUpdate()
    {
        Move ();
        
        Turning();

    }

    
    void Move()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addMouseClicks();
        }
        if (Input.GetMouseButton(0))
        {
            
            player.velocity = transform.forward * speed;
            anim.SetBool("Running", true);
        }
        else
        {
            player.velocity = new Vector3(0f,0f,0f);
            anim.SetBool("Running", false);
        }
        
        
    }
    
    // From survival shooter tutorial
    // https://unity3d.com/learn/tutorials/s/survival-shooter-tutorial
    void Turning ()
    {
        
        Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit floorHit;
        
        if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
        {
            
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;
            Quaternion newRotation = Quaternion.LookRotation (playerToMouse);
            player.MoveRotation (newRotation);
        }
    }


}