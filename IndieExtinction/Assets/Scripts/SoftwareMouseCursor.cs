using UnityEngine;
using System.Collections;  

public class SoftwareMouseCursor : MonoBehaviour  
 {
    // The texture for the mouse cursor  
    public Texture2D _mouseCursorTexture;// = (Texture2D)Resources.Load("Textures/hand_default.png");

    // Default GUIStyle needs a modification to properly display  
    // the mouse cursor wihtin a Label  
    GUIStyle _mouseCursorStyle;
    
    void Start()  
    {
        //Screen.showCursor = false;
        
        // If the mouse cursor texture is not set within the Unity Editor
        /*if (_mouseCursorTexture == null)
        {
            _mouseCursorTexture = (Texture2D)Resources.Load("Textures/hand_default");
        }*/
        _mouseCursorTexture.Resize(_mouseCursorTexture.width / 10, _mouseCursorTexture.height / 10);
        _mouseCursorStyle = new GUIStyle();
        _mouseCursorStyle.padding = new RectOffset(); // Clear the default padding  
     }  
    
     /// <summary>  
     /// Draw the GUI and the mouse cursor
     /// </summary>
     private void OnGUI()
     {
         // Determine the position of the software mouse cursor based on the mouse inputs
         // Note that the vertical position needs to be inverted since the mouse position
         // (0,0) is at the bottom-left of the screen while the GUI position (0, 0) is at
         // the top-left of the screen.
         //Rect mousePosition = new Rect(Input.mousePosition.x,  Screen.height - Input.mousePosition.y, _mouseCursorTexture.width, 
         //    _mouseCursorTexture.height);
         Rect mousePosition = new Rect(Input.mousePosition.x - _mouseCursorTexture.width,
             Screen.height - Input.mousePosition.y - _mouseCursorTexture.height, 
             _mouseCursorTexture.width,
             _mouseCursorTexture.height);
         GUI.DrawTexture(mousePosition, _mouseCursorTexture);
     }
    
     /// <summary>
     /// Draw the mouse cursor after OnGUI has completed.  This ensures
     /// that the mouse cursor is drawn above any GUI.Window().
     /// </summary>
     /// <param name="mousePosition">The position of the mouse cursor</param>
     /// <returns>Coroutine instance</returns>
     private IEnumerator DrawMouseCursor(Rect mousePosition)
     {
         // Wait until OnGUI() has been called
         yield return new WaitForEndOfFrame();   

         // Requires Unity Pro
         Graphics.DrawTexture(mousePosition, _mouseCursorTexture);
     }
}