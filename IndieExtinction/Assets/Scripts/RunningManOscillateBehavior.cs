using UnityEngine;
using System.Collections;

public class RunningManOscillateBehavior : BillboardBehavior 
{
    public float oscillationsPerSecond = 1.4f;

	public override void Start () 
    {
        base.Start();
	}

    public override void Update() 
    {
        roll = 15 + 30 * Mathf.Sin(Time.time * Mathf.PI * 2 * oscillationsPerSecond);

        base.Update();
	}
}
