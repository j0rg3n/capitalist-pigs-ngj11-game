using UnityEngine;
using System.Collections;

public class GlobalGameStateBehavior : MonoBehaviour
{
    public bool gameScene;
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
        startTime = Time.time;

        score = 100;

        GetComponent<AudioSource>().playOnAwake = !gameScene;
	}
	
	void Update () 
    {
        if (!gameScene)
        {
            float t = Time.time - startTime;

            int slideIndex = slideTimes.Length - 1;
            for (int i = 0; i < slideTimes.Length - 1; ++i)
            {
                if (t >= slideTimes[i] && t < slideTimes[i + 1])
                {
                    slideIndex = i - 1;
                    break;
                }
            }

            SetSlideIndex(slideIndex);
        }
	}

    private void SetSlideIndex(int newSlideIndex)
    {
        if (prevSlideIndex != newSlideIndex)
        {
            var projector = GlobalObjects.GetSlideProjector();
            projector.SetSlide(newSlideIndex);
            prevSlideIndex = newSlideIndex;
        }
    }

    private float startTime;
    private int prevSlideIndex = -1;
    private float[] slideTimes = new float[] { 0, 1, 5, 8, 12, 15 };
}
