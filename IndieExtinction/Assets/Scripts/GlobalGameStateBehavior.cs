using UnityEngine;
using System.Collections;

public class GlobalGameStateBehavior : MonoBehaviour
{
    public bool gameScene;
    public int score;
    public int kill = 10;

    public AudioClip introClip;
    public AudioClip inGameClip;
    public Object nextScene;

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
            if (!themeAudioSource.isPlaying)
            {
                return 0;
            }

            return (theAudioClip.samples - themeAudioSource.timeSamples) / theAudioClip.frequency;
        }
    }

    public float TimeElapsed
    {
        get
        {
            var themeAudioSource = GetComponent<AudioSource>();
            if (!themeAudioSource.isPlaying)
            {
                return theAudioClip.samples / theAudioClip.frequency;
            }

            return themeAudioSource.timeSamples / theAudioClip.frequency;
        }
    }

    void Start() 
    {
        score = 100;

        theAudioClip = gameScene ? inGameClip : introClip;
        GetComponent<AudioSource>().clip = theAudioClip;
        GetComponent<AudioSource>().Play();
	}
	
	void Update () 
    {
        if (!gameScene)
        {
            float t = TimeElapsed;

            int slideIndex = -1;
            for (int i = 0; i < slideTimes.Length - 1; ++i)
            {
                if (t >= slideTimes[i] && t < slideTimes[i + 1])
                {
                    slideIndex = i - 1;
                    break;
                }
            }

            if (slideIndex != -1)
            {
                SetSlideIndex(slideIndex);
            }
            else
            {
                Application.LoadLevel(1);
            }
        }
	}

    private void SetSlideIndex(int newSlideIndex)
    {
        if (prevSlideIndex != newSlideIndex)
        {
            var projector = GlobalObjects.GetSlideProjector();
            projector.SetSlide(newSlideIndex, newSlideIndex >= 0 ? slideZooms[newSlideIndex] : 0);
            prevSlideIndex = newSlideIndex;
        }
    }

    private AudioClip theAudioClip;
    private int prevSlideIndex = -1;

    // Slide 0 is the pause before the first actual slide.
    private float[] slideTimes = new float[] { 0,  0,  7, 14, 21,  26 };
    private float[] slideZooms = new float[] {     0, -2, -1,  3,  2  };
}