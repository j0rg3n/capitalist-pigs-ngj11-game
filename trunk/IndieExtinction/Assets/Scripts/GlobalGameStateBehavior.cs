using UnityEngine;
using System.Collections;
using Irrelevant.Assets.Scripts;

public class GlobalGameStateBehavior : MonoBehaviour
{
    public int score;
    public int kill = 10;
    public float pie = 100;
    public float instanceScale = 2;

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
        lastPieLossTime = Time.time;
    }

    public void ScaleInstance(Transform newInstance)
    {
        newInstance.localScale = newInstance.localScale * GlobalObjects.GetGlobbalGameState().instanceScale;
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

    // TODO: Move VisualPie into guiScriptBehavior.
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

        nextWaveTime = Time.time + MUSIC_WINDUP_TIME;
	}
	
	void Update () 
    {
        if (gameScene)
        {
            int houseCount = 0;
            foreach (IndieHouseLocation location in GlobalObjects.indieHouseLocations)
            {
                if (location.isPresent)
                {
                    ++houseCount;
                }
            }

            var healtPie = GlobalObjects.GetHealthPie();
            healtPie.amount = 1 - VisualPie / 100.0f;

            float now = Time.time;
            if (now > nextWaveTime)
            {
                int nextWaveCount = waveSizes[Mathf.Min(nextWaveIndex, waveSizes.Length - 1)];
                nextWaveTime = now + waveTimes[Mathf.Min(nextWaveIndex, waveTimes.Length - 1)];
                GlobalObjects.GetStudio().SpawnWave(nextWaveCount);
                ++nextWaveIndex;
            }

            string newAlert = null;
            bool newAlertFlashing = false;
            if (nextWaveTime - now < ALERT_WAVE_SECONDS)
            {
                newAlert = string.Format("Cutbacks due in {0} seconds...", Mathf.CeilToInt(nextWaveTime - now));
            }
            else if (now - lastPieLossTime < PIE_LOSS_ALERT_TIME)
            {
                newAlert = string.Format("You lost another market share!");
                newAlertFlashing = true;
            }
            else if (houseCount > 0)
            {
                newAlert = "Click the houses!";
                newAlertFlashing = true;
            }

            GlobalObjects.GetGUIScriptBehavior().alert = newAlert;
            GlobalObjects.GetGUIScriptBehavior().alertFlashing = newAlertFlashing;
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

    private const int ALERT_WAVE_SECONDS = 3;
    private const int PIE_LOSS_ALERT_TIME = 3;
    private const float MUSIC_WINDUP_TIME = 8;

    private float lastPieLossTime = float.MinValue;
    private bool gameScene;
    private AudioClip theAudioClip;
    private int prevSlideIndex = -1;
    private AnimationCurve visualPieCurve;
    private float nextWaveTime = float.MaxValue;
    private int nextWaveIndex = 0;

    private int[] waveSizes = new int[] { 16, 32, 32, 32, 40 };
    private int[] waveTimes = new int[] { 16, 12, 9, 8,  };

    // Slide 0 is the pause before the first actual slide.
    private float[] slideTimes = new float[] { 0,  0,  7, 14, 21,  26 };
    private float[] slideZooms = new float[] {     0, -2, -1,  3,  2  };
}
