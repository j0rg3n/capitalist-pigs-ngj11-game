using UnityEngine;
using System.Collections;
using Irrelevant.Assets.Scripts;

public class GlobalGameStateBehavior : MonoBehaviour
{
    public int score;
    public int kill = 10;
    public float pie = 100;
    public float instanceScale = 2;
    public int houseCount;

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

    public void LoadIntroLevel()
    {
        persistentGameState.LoadIntro();
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
                GameOver = true;
                return;
            }
        }
    }

    public bool GameOver 
    {
        get { return gameOver; }
        set 
        { 
            gameOver = value;
            gameOverTime = Time.time;
        } 
    }

    public float SecondsSinceGameOver
    {
        get { return gameOver ? Time.time - gameOverTime : float.MinValue; }
    }

    private float gameOverTime;

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
			if (nonMusicTimer)
			{
				return Time.time - musicStopTime + theAudioClip.samples / theAudioClip.frequency;
			}

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
        score = 0;

        theAudioClip = gameScene ? inGameClip : introClip;
        GetComponent<AudioSource>().clip = theAudioClip;
        GetComponent<AudioSource>().Play();

        nextWaveTime = Time.time + MUSIC_WINDUP_TIME;
	}
	
	void Update () 
    {
        if (gameScene)
        {
            houseCount = 0;
            //(foreach (IndieHouseLocation location in GlobalObjects.indieHouseLocations)
            if (!GameOver)
            {
                houseCount = 0;
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
                if (persistentGameState.IsLastWave(nextWaveIndex - 1))
                {
                    persistentGameState.LoadNextLevel();
                    return;
                }

                int nextWaveCount = persistentGameState.GetWaveSize(nextWaveIndex);
                nextWaveTime = now + persistentGameState.GetWaveTime(nextWaveIndex);
                GlobalObjects.GetStudio().SpawnWave(nextWaveCount);
                ++nextWaveIndex;
            }

                string newAlert = null;
                bool newAlertFlashing = false;
                if (nextWaveTime - now < ALERT_WAVE_SECONDS)
                {
                    newAlert = string.Format(CheeringMessages.GetMessage(MessageType.Wave), Mathf.CeilToInt(nextWaveTime - now));
                }
                else if (now - lastPieLossTime < PIE_LOSS_ALERT_TIME)
                {
                    newAlert = CheeringMessages.GetMessage(MessageType.Loss);
                    newAlertFlashing = true;
                }
                else if (houseCount > 0)
                {
                    newAlert = CheeringMessages.GetMessage(MessageType.Click);
                    newAlertFlashing = true;
                }

                GlobalObjects.GetGUIScriptBehavior().alert = newAlert;
                GlobalObjects.GetGUIScriptBehavior().alertFlashing = newAlertFlashing;

                if (TimeRemaining < 0.0f)
                    GameOver = true;
            }
        }
        else
        {
			var themeAudioSource = GetComponent<AudioSource>();
			if (!themeAudioSource.isPlaying && !nonMusicTimer)
			{
				nonMusicTimer = true;
				musicStopTime = Time.time;
			}

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
            projector.SetSlide(newSlideIndex, newSlideIndex >= 0 ? slideZooms[Mathf.Min(newSlideIndex, slideZooms.Length - 1)] : 0);
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
    public bool gameOver = false;

	private bool nonMusicTimer;
	private float musicStopTime;

    // Slide 0 is the pause before the first actual slide.
    private float[] slideTimes = new float[] { 0,  
        0,     // credits
        3,     // 
        6.5f,     // graph
        10.25f,     // thinking
        14,  // window looking out 
        16,  // closeup 1
        17.5f,    // closeup 2
        20.5f,    // I've got it!
        22.5f,  // Redundancies!
        25,    // Title
        30 };
    private float[] slideZooms = new float[] { 0, -2, -1,  0,  0  };

    private static PersistentGameState persistentGameState = new PersistentGameState();
}
