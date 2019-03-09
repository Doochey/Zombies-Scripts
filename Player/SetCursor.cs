using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursor : MonoBehaviour
{
    public Texture2D cursor;

    void Start()
    {
        // Set cursor at game start
        Cursor.SetCursor(cursor, new Vector2(0, 0), CursorMode.Auto);
    }

    // Resets cursor to default
    public void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); 
    }

}
