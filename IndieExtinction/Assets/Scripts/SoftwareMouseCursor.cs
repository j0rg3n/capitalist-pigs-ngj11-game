using UnityEngine;
using System.Collections;  

public class SoftwareMouseCursor : MonoBehaviour  
 {
    // The texture for the mouse cursor
    public Texture2D[] texArray;
    Texture2D cursorTex;
    public float Scale = 0.2f;


    private Vector2 OnClickOffset =  new Vector2(8,6);
    private Vector2 CurOffset;
    public float offsetTime = 0.5f;
    private float times = 0.0f;
    bool clicked = false;
    bool peaked = false;
    void Start()  
    {
        Screen.showCursor = false;
        cursorTex = texArray[0];
     }  
    
     /// <summary>  
     /// Draw the GUI and the mouse cursor
     /// </summary>
     private void OnGUI()
     {
         if (clicked)
         {
             if (!peaked)
             {
                 times += Time.deltaTime;
                 //cursorTex = texArray[0];
             }
             else
             {
                 //cursorTex = texArray[1];
                 times -= Time.deltaTime;
             }

             CurOffset =( OnClickOffset * (times/offsetTime));
             if (times > offsetTime)
             {
                peaked = true;
                
             }
             if (peaked && times < 0.0f)
             {
                 CurOffset = Vector2.zero;
                 peaked = false;
                 clicked = false;
                 times = 0.0f;
             }
         }

         if (Event.current.type == EventType.MouseDown)
         {
             cursorTex = texArray[0];
             clicked = true;
         }

         Vector3 mousePos = Input.mousePosition;
         Rect pos = new Rect(mousePos.x - (float)(0.88f * cursorTex.width * Scale) + CurOffset.x, (Screen.height - mousePos.y) - (int)(0.55f * Scale * cursorTex.height)+CurOffset.y, (int)(cursorTex.width * Scale), (int)(cursorTex.height * Scale));
         GUI.Label(pos, cursorTex);
     }
}