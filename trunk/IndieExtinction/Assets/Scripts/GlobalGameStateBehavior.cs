using UnityEngine;
using System.Collections;

public class GlobalGameStateBehavior : MonoBehaviour
{
    public int score;
    public int kill = 10;
    public float pie = 100;

    public AudioClip introClip;
    public AudioClip inGameClip;
    public AudioClip gameDevEnded;
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

    public void GameDeveloped()
    {
        Pie -= 10;
        audio.PlayOneShot(gameDevEnded);
    }

    private float Pie
    {
        get { return pie; }
        set 
        {
            float now = Time.time;
            visualPieCurve = AnimationCurve.EaseInOut(now, VisualPie, now + .3f, value);
            pie = value;

            if (pie <= 0)
            {
                //  TOD: Load end fo game scjlien.
                Application.LoadLevel(0);
            }
        }
    }

    public float VisualPie
    {
        get { return visualPieCurve != null ? visualPieCurve.Evaluate(Time.time) : pie; }
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

    public bool IsGameScene
    {
        get { return gameScene; }
    }

    void Start() 
    {
        gameScene = GlobalObjects.GetSlideProjector() == null;

        score = 100;

        theAudioClip = gameScene ? inGameClip : introClip;
        GetComponent<AudioSource>().clip = theAudioClip;
        GetComponent<AudioSource>().Play();
	}
	
	void Update () 
    {
        if (gameScene)
        {
            var healtPie = GlobalObjects.GetHealthPie();
            healtPie.amount = 1 - VisualPie / 100.0f;
        }
        else
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

    private bool gameScene;
    private AudioClip theAudioClip;
    private int prevSlideIndex = -1;
    private AnimationCurve visualPieCurve;

    // Slide 0 is the pause before the first actual slide.
    private float[] slideTimes = new float[] { 0,  0,  7, 14, 21,  26 };
    private float[] slideZooms = new float[] {     0, -2, -1,  3,  2  };
}
