using UnityEngine;

public abstract class StudioBehaviorBase : MonoBehaviour
{
    public Transform indieDevFemalePrefab;
    public Transform indieDevMalePrefab;

    protected void SpawnIndieDevs(int count, int indieStudioAiTileInd)
    {
		var worldSpawnPos = transform.position;

        for (int i = 0; i < count; ++i)
        {
            Transform prefab = Random.value > 0.35 ? indieDevMalePrefab : indieDevFemalePrefab;

            Transform newDev = (Transform)Instantiate(prefab, worldSpawnPos, Quaternion.identity);
            GlobalObjects.GetGlobbalGameState().ScaleInstance(newDev);

            IndieDevBehavior devGuy = newDev.GetComponent<IndieDevBehavior>();
			devGuy.aiDevGuy.SetLastHouse(indieStudioAiTileInd);
		}
    }

}
