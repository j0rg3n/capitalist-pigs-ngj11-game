using UnityEngine;
using System.Collections;

public class guiScriptBehavior : MonoBehaviour 
{
        float startTime = 120.0f;
        float timeRemaining = 0.0f;
        string score = "Score: ";
        string countDown;
        int minutes;
        int seconds;
    void OnGUI()
    {
        timeRemaining= startTime - Time.time;
        minutes = ((int)timeRemaining / 60);
        seconds = ((int)timeRemaining % 60);
        countDown = minutes.ToString() + ":" + seconds.ToString("D2");
        var globalgamestate = (GlobalGameStateBehavior)Object.FindObjectOfType(typeof(GlobalGameStateBehavior));
        int cash = globalgamestate.getscore;
        GUI.Label(new Rect(10, 10, 200, 20), score + cash);
        GUI.Label(new Rect(50, 50, 200, 20), countDown);
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
