using UnityEngine;
using System.Collections;

public class guiScriptBehavior : MonoBehaviour 
{
    string score = "Score: ";
    string countDown;
    int minutes;
    int seconds;
    public string alert;
    public bool alertFlashing;
    public Texture buttonTexture;
    public Font ft;

    void OnGUI()
    {
        if (!GetComponent<GlobalGameStateBehavior>().IsGameScene)
        {
            return;
        }

        GUI.skin.font = ft;
        
        var globalgamestate = (GlobalGameStateBehavior)Object.FindObjectOfType(typeof(GlobalGameStateBehavior));

        var timeRemaining = globalgamestate.TimeRemaining;
        minutes = ((int)timeRemaining / 60);
        seconds = ((int)timeRemaining % 60);
        countDown = minutes.ToString() + ":" + seconds.ToString("D2");
        int cash = globalgamestate.Score;
        //GUI.Label(new Rect(10, 10, 200, 20), score + cash);
        GUI.Label(new Rect(5, 35, 200, 100), countDown);
        GUILayout.Label(score + cash);
        
        /*
        if (GUI.Button(new Rect(10, 100, 40, 40), buttonTexture))
        {
            Debug.Log("Clicked the button with an image");
        }
        */

        if (alert != null && (!alertFlashing || ((int)(Time.time * 3) & 1) == 0))
        {
            GUI.Label(new Rect(630, 5, 200, 200), alert);
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
