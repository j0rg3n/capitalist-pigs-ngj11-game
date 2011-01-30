using UnityEngine;

public abstract class StudioBehaviorBase : MonoBehaviour
{
    public Transform indieDevFemalePrefab;
    public Transform indieDevMalePrefab;

    protected void SpawnIndieDevs(int count, int indieStudioAiTileInd)
    {
		var worldSpawnPos = transform.position;
        //var studioBounds = GetComponent<MeshFilter>().mesh.bounds;

        for (int i = 0; i < count; ++i)
        {
            //var localSpawnPos = RandomUtil.GetPointInBounds(studioBounds);
            //var worldSpawnPos = transform.TransformPoint(localSpawnPos);
            //Transform indieDvInstance = (Transform)Instantiate(indieDevPrefab, worldSpawnPos, Quaternion.identity);
			Transform newDev;
            if (Random.value > 0.35)
            {
				newDev = (Transform)Instantiate(indieDevMalePrefab, worldSpawnPos, Quaternion.identity);
            }
            else
            {
				newDev = (Transform)Instantiate(indieDevFemalePrefab, worldSpawnPos, Quaternion.identity);
            }
			IndieDevBehavior devGuy = newDev.GetComponent<IndieDevBehavior>();
			devGuy.aiDevGuy.SetLastHouse(indieStudioAiTileInd);
		}
    }

}
