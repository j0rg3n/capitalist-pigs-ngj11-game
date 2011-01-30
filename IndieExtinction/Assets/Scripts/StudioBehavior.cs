using UnityEngine;
using System.Collections;
using Irrelevant.Assets.Scripts;

public class StudioBehavior : StudioBehaviorBase
{
	public void SpawnWave(int count)
	{
        SpawnIndieDevs(count, -1);
    }
}
