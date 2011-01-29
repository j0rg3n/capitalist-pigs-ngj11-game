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
            var localSpawnPos = RandomUtil.GetPointInBounds(studioBounds);
            var worldSpawnPos = transform.TransformPoint(localSpawnPos);

            Transform indieDevInstance = (Transform)Instantiate(indieDevPrefab, worldSpawnPos, Quaternion.identity);
        }

        Destroy (gameObject);
    }

    public void Update() 
    {
	
	}
}
