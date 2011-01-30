using UnityEngine;
using System.Collections;
using Irrelevant.Assets.Scripts;

public class StudioBehavior : StudioBehaviorBase
{

	public void OnMouseClicked(IndexedHit hitInfo)
	{
        SpawnIndieDevs(75, -1);
    }
}
