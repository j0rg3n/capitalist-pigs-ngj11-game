using UnityEngine;
using System.Collections;

public class GlobalObjects
{
    public static Camera GetMainCamera()
    {
        return GameObject.Find(MAIN_CAMERA).GetComponent<Camera>(); // GameObject.Find(ObjectNames.MAIN_CAMERA).GetComponent<Camera>();
    }

    // Marks the class as 'static'. 
    // Mono doesn't allow the 'static' modifier on classes.
    private GlobalObjects()
    {
    }

    private const string MAIN_CAMERA = "Main Camera";
}
