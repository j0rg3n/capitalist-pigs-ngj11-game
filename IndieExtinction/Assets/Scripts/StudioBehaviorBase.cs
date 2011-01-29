using UnityEngine;

public abstract class StudioBehaviorBase : MonoBehaviour
{
    public Transform indieDevFemalePrefab;
    public Transform indieDevMalePrefab;
    public AudioClip Sound_explotion;

    protected void SpawnIndieDevs(int count)
    {
        var studioBounds = GetComponent<MeshFilter>().mesh.bounds;

        for (int i = 0; i < count; ++i)
        {
            var localSpawnPos = RandomUtil.GetPointInBounds(studioBounds);
            var worldSpawnPos = transform.TransformPoint(localSpawnPos);
            //Transform indieDvInstance = (Transform)Instantiate(indieDevPrefab, worldSpawnPos, Quaternion.identity);
            if (Random.value > 0.35)
            {
                Instantiate(indieDevMalePrefab, worldSpawnPos, Quaternion.identity);
            }
            else
            {
                Instantiate(indieDevFemalePrefab, worldSpawnPos, Quaternion.identity);
            }
        }
    }

}
