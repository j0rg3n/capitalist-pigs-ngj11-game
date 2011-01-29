using UnityEngine;
using System.Collections;

public class IndieStudioBehavior : StudioBehaviorBase
{
    public int indieDevCount = 50;
    public float devTimeSeconds = 5;
    
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
            Destroy(gameObject);
            return;
        }

        var pieChart = GetComponent<PieChartGUIBehavior>();
        pieChart.amount = elapsed / devTimeSeconds;
	}

    private float startTime;
}
