using UnityEngine;
using System.Collections;

public class RunningManOscillateBehavior : BillboardBehavior 
{
    public float oscillationsPerSecond = 1.4f;

	public override void Start () 
    {
        base.Start();
	}

    public void OnMouseClicked()
    {
        oscillationStarted = Time.time;
    }

    public override void Update() 
    {
        if (Time.time - oscillationStarted < 1)
        {
            roll = 15 + 30 * Mathf.Sin(Time.time * Mathf.PI * 2 * oscillationsPerSecond);
        }

        base.Update();
	}

    private float oscillationStarted = float.MinValue;
}
