using UnityEngine;
using System.Collections;
using Irrelevant.Assets.Scripts;

public class IndieStudioBehavior : StudioBehaviorBase
{
    public int indieDevCount = 50;
    public float devTimeSeconds = 5;

	public IndieHouseLocation location;

    void Start() 
    {
        startTime = Time.time;
	}
	
	void Update () 
    {
        float elapsed = Time.time - startTime;
        
        if (elapsed > devTimeSeconds)
        {
            SpawnIndieDevs(indieDevCount);
			location.HouseDestroyed();
            Destroy(gameObject);
            return;
        }

        var pieChart = GetComponent<PieChartGUIBehavior>();
        pieChart.amount = elapsed / devTimeSeconds;
	}

    private float startTime;
}
