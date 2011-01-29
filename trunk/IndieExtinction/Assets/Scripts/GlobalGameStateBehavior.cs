using UnityEngine;
using System.Collections;

public class GlobalGameStateBehavior : MonoBehaviour
{
    public int score;
    public int kill = 10;

    public void addkillscore()
    {
        score += kill;
    }

    public void multiplayer(int goodPie)
    {
        score *= goodPie;
    }

    public void demultiplyer(int badPie)
    {
        score *= badPie;
    }

    public int Score
    {
        get { return score; }
    }

    public float TimeRemaining
    {
        get
        {
            var themeAudioSource = GetComponent<AudioSource>();
            return themeAudioSource.clip.length - themeAudioSource.time;
        }
    }

	void Start () 
    {
        score = 100;
	}
	
	void Update () 
    {
	    
	}
}
