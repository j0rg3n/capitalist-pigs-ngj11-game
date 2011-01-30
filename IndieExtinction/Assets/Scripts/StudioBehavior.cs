using UnityEngine;
using System.Collections;
using Irrelevant.Assets.Scripts;

public class StudioBehavior : StudioBehaviorBase
{

	public void OnMouseClicked(RaycastHit hitInfo)
	{
        SpawnIndieDevs(10, -1);

        audio.PlayOneShot(Sound_explotion);
        StartCoroutine(DestroyAfterDelay(Sound_explotion.length));
    }

    public IEnumerator DestroyAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}