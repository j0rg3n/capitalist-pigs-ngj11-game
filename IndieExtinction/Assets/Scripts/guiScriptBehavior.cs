using UnityEngine;
using System.Collections;

public class guiScriptBehavior : MonoBehaviour 
{

    string score = "Score: ";
    void OnGUI()
    {
       var globalgamestate = (GlobalGameStateBehavior)Object.FindObjectOfType(typeof(GlobalGameStateBehavior));
       int cash = globalgamestate.getScore();
       GUI.Label(new Rect(10, 10, 200, 20), score + cash);
        /*
        if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
        {
            print("You clicked the button!");
        }
        */
    }

	// Use this for initialization
	void Start () 
    {
    
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
