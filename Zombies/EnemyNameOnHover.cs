using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyNameOnHover : MonoBehaviour
{
    public GameObject enemyHoverText;
    
    private LayerMask enemyLayer;
    // Start is called before the first frame update
    void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit enemyHit;
        
        if(Physics.Raycast (camRay, out enemyHit, Mathf.Infinity, enemyLayer))
        {

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
}
