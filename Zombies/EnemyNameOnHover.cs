using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyNameOnHover : MonoBehaviour
{
    public GameObject enemyHoverText; // The text to go under the cursor
    
    private LayerMask enemyLayer; // The layermask of teh enemies

    private GameMaster GM;
    
    void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        GM = GameObject.FindWithTag("GM").GetComponent<GameMaster>();
    }

    
    void Update()
    {
        DisplayNameOnHover();
        
    }

    // Casts ray through cursor, if hits enemy, displays tag of enemy under cursor
    void DisplayNameOnHover()
    {
        if (!GM.isGameOver())  // Should not display during game over screens
        {
            Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit enemyHit;
        
            if(Physics.Raycast (camRay, out enemyHit, Mathf.Infinity, enemyLayer))
            {

                // Adjust text transform to under cursor
                Vector3 underCursor = Input.mousePosition;
                underCursor.x -= 5;
                underCursor.y -= 40;
            
                enemyHoverText.GetComponent<Transform>().position = underCursor;
                enemyHoverText.GetComponent<Text>().text = enemyHit.transform.gameObject.tag;
                enemyHoverText.SetActive(true);
            }
            else
            {
                enemyHoverText.SetActive(false);
            }  
        }
        else
        {
            enemyHoverText.SetActive(false); 
        }
    }
}
