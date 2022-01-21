using UnityEngine;

public class CursorEngine : MonoBehaviour
{
    public Texture2D cursor, cursorQuestion, cursorExclamation;
    
    void Start()
    {
        Cursor.SetCursor(cursor, new Vector2(0,0), CursorMode.Auto);
    }
    
}
