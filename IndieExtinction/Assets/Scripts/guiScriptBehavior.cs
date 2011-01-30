using UnityEngine;
using System.Collections;

public class guiScriptBehavior : MonoBehaviour 
{
    string score = "Score: ";
    string countDown;
    int minutes;
    int seconds;
    public Texture buttonTexture;

    void OnGUI()
    {
        if (!GetComponent<GlobalGameStateBehavior>().IsGameScene)
        {
            return;
        }

        var globalgamestate = (GlobalGameStateBehavior)Object.FindObjectOfType(typeof(GlobalGameStateBehavior));

        var timeRemaining = globalgamestate.TimeRemaining;
        minutes = ((int)timeRemaining / 60);
        seconds = ((int)timeRemaining % 60);
        countDown = minutes.ToString() + ":" + seconds.ToString("D2");
        int cash = globalgamestate.Score;
        GUI.Label(new Rect(10, 10, 200, 20), score + cash);
        GUI.Label(new Rect(50, 50, 200, 20), countDown);

        if (GUI.Button(new Rect(10, 100, 40, 40), buttonTexture))
        {
            Debug.Log("Clicked the button with an image");
        }
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
