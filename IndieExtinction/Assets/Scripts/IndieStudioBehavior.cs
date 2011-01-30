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
	}
	
	void Update () 
    {
        var basePosition = MathUtil.GetBasePointWithAlignment(gameObject, new Vector2(.5f, 0));
        var label = transform.GetChild(0).GetComponent<GUIText>();
        label.text = string.Format("0x{0:X2}", indieDevCount);
        label.transform.position = new Vector2(basePosition.x / Screen.width,
            basePosition.y / Screen.height);

        var development = Development;
        if (development >= 1)
        {
			System.Diagnostics.Debug.Assert(location.houseTileInd >= 0);

            GlobalObjects.GetGlobbalGameState().Pie -= 10;
            SpawnIndieDevs(indieDevCount, location.houseTileInd);
			location.HouseDestroyed();
            Destroy(gameObject);
            return;
        }

        var pieChart = GetComponent<PieChartGUIBehavior>();
        pieChart.amount = development;
	}

    private float startTime;
    private float startDevelopment;
    private int indieDevCount = 0;
}
