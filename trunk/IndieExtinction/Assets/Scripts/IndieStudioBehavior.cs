using UnityEngine;
using System.Collections;
using Irrelevant.Assets.Scripts;

public class IndieStudioBehavior : StudioBehaviorBase
{
    /// <summary>
    /// Time taken for one developer to finish a game.
    /// </summary>
    public float devTimeSeconds = 60;

    public IndieHouseLocation location;
    public AudioClip[] Explotions;
    public AudioClip pop;

    public int IndieDevCount
    {
        get { return indieDevCount; }
        set 
        { 
            startDevelopment = Development;
            startTime = Time.time;
            indieDevCount = value;
        }
    }

    public float Development
    {
        get { return startDevelopment + (Time.time - startTime) / devTimeSeconds * indieDevCount; }
    }

    void Start() 
    {
        startTime = Time.time;
        audio.PlayOneShot(pop);
	}
	
	void Update() 
    {
        var basePosition = MathUtil.GetBasePointWithAlignment(gameObject, new Vector2(.5f, 0));
        var label = transform.GetChild(0).GetComponent<GUIText>();
        label.text = indieDevCount.ToString();
        label.transform.position = new Vector2(basePosition.x / Screen.width,
            basePosition.y / Screen.height);

        if (!isSetForDestruction)
        {
            var development = Development;
            if (development >= 1)
            {
                System.Diagnostics.Debug.Assert(location.houseTileInd >= 0);

                GlobalObjects.GetGlobbalGameState().GameDeveloped();
                SpawnIndieDevs(indieDevCount, location.houseTileInd);
                location.HouseDestroyed();
                Destroy(gameObject);
                return;
            }

            var pieChart = GetComponent<PieChartGUIBehavior>();
            pieChart.amount = development;
        }
	}

    public void OnMouseClicked(RaycastHit hitInfo)
    {
        if (!isSetForDestruction)
        {
            IndieDevCount--;
            System.Diagnostics.Debug.Assert(location.houseTileInd >= 0);

            SpawnIndieDevs(1, location.houseTileInd);
            if (indieDevCount == 0)
            {
                isSetForDestruction = true;
                PlayRandomExplotionSound();
            }
        }
    }

    private void PlayRandomExplotionSound()
    {
        if (Explotions.Length > 0)
        {
            AudioClip explotion = Explotions[Random.Range(0, Explotions.Length)];
            if (explotion != null)
            {
                audio.PlayOneShot(explotion);
                StartCoroutine(DestroyAfterDelay(explotion.length));
            }
        }
    }

    private IEnumerator DestroyAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
		location.HouseDestroyed();
        Destroy(gameObject);
    }

    private float startTime;
    private float startDevelopment;
    private int indieDevCount = 0;

    private bool isSetForDestruction = false;
}
