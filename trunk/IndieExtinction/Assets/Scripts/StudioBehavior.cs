using UnityEngine;
using System.Collections;

public class StudioBehavior : MonoBehaviour 
{
    public Transform indieDevPrefab;

    public void Start () 
    {
	}

    public void OnMouseClicked(RaycastHit hitInfo)
    {
        var studioBounds = GetComponent<MeshFilter>().mesh.bounds;

        for (int i = 0; i < 20; ++i)
        {
            var localSpawnPos = GetRandomPos(studioBounds);
            var worldSpawnPos = transform.TransformPoint(localSpawnPos);

            Transform indieDevInstance = (Transform)Instantiate(indieDevPrefab, worldSpawnPos, Quaternion.identity);
        }

        Destroy (gameObject);
    }

    private static Vector3 GetRandomPos(Bounds bounds)
    {
        float x = Random.RandomRange(bounds.center.x, bounds.center.x + bounds.extents.x);
        float y = Random.RandomRange(bounds.center.x, bounds.center.x + bounds.extents.x);
        float z = Random.RandomRange(bounds.center.x, bounds.center.x + bounds.extents.x);
        return new Vector3(x, y, z);
    }

    public void Update() 
    {
	
	}
}
