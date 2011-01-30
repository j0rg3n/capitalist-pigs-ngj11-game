using UnityEngine;
using System.Collections;
using Irrelevant.Assets.Scripts;

public class StudioBehavior : StudioBehaviorBase
{

	public void OnMouseClicked(RaycastHit hitInfo)
	{
        SpawnIndieDevs(75, -1);
    }
}
