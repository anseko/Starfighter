using UnityEngine;

public class CursorEngine : MonoBehaviour
{
    public Texture2D cursor, cursorQuestion, cursorExclamation;
    
    void Start()
    {
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    }
    
}
