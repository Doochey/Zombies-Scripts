using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    // Stat Logging should persist over game restarts
    
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
