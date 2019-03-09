using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 8f; // Speed of player    
    Rigidbody player; // Player object         
    private Animator anim;
    private bool moving; // If player is moving
    int floorMask; // Layermask of floor

    void Start()
    {
        player = GetComponent <Rigidbody> ();
        anim = GetComponent<Animator>();
        
        floorMask = LayerMask.GetMask("Floor");
    }


    void FixedUpdate()
    {
        Move();
        
        Turning();

    }

    
    void Move()
    {
        // If mouse button pressed down
        if (Input.GetMouseButtonDown(0))
        {
            // Log mouse click action
            GameObject.FindWithTag("StatLog").GetComponent<StatLogging>().addMouseClicks();
        }
        
        // If mouse button held down
        if (Input.GetMouseButton(0))
        {
            // Move player forward
            player.velocity = transform.forward * speed;
            
            // Play running animation
            anim.SetBool("Running", true);
        }
        else
        {
            // Stop player moving
            player.velocity = new Vector3(0f,0f,0f);
            
            // Stop running animation (Go to idle animation)
            anim.SetBool("Running", false);
        }
        
        
    }
    
    // From survival shooter tutorial
    // https://unity3d.com/learn/tutorials/s/survival-shooter-tutorial
    void Turning ()
    {
        // Raycast through cursor, if hits floor, face player towards floor
        Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit floorHit;
        
        if(Physics.Raycast (camRay, out floorHit, Mathf.Infinity, floorMask))
        {
            
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            player.MoveRotation(newRotation);
        }
    }


}