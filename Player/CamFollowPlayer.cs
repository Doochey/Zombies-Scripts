using UnityEngine;

public class CamFollowPlayer : MonoBehaviour
{
    public Transform target; // The transform for the camera to follow       
    public float smoothing = 5f; // Smoothing factor

    Vector3 offset; // Offset between camera and player               

    void Start()
    {
        offset = transform.position - target.position;
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 targetCamPos = target.position + offset;

            // Move camera to follow target
            transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
        
    }
}
