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
        var basePosition = MathUtil.GetBasePointWithAlignment(gameObject, new Vector2(.5f, 0));
        var label = transform.GetChild(0).GetComponent<GUIText>();
        label.text = string.Format("0x{0:X2}", indieDevCount);
        label.transform.position = new Vector2(basePosition.x / Screen.width,
            basePosition.y / Screen.height);

        float elapsed = Time.time - startTime;        
        if (elapsed > devTimeSeconds)
        {
			System.Diagnostics.Debug.Assert(location.houseTileInd >= 0);
            SpawnIndieDevs(indieDevCount, location.houseTileInd);
			location.HouseDestroyed();
            Destroy(gameObject);
            return;
        }

        var pieChart = GetComponent<PieChartGUIBehavior>();
        pieChart.amount = elapsed / devTimeSeconds;
	}

    private float startTime;
}
