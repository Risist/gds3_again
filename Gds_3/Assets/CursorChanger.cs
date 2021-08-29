using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChanger : MonoBehaviour
{
    public Texture2D crosshair = null;
    public float cursorHeight = 2;
    public float cursorWeight =2;
    void Start()
    {
        if(!crosshair)
        {
            return;
        }
        Vector2 cursorOffset = new Vector2(crosshair.width / cursorWeight, crosshair.height / cursorHeight);
        Cursor.SetCursor(crosshair, cursorOffset, CursorMode.Auto);
    }
}
