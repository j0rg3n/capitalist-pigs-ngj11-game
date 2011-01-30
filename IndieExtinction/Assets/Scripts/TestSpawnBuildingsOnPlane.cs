using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestSpawnBuildingsOnPlane : MonoBehaviour 
{
    public Transform buildingPrefab1 = null;
    public Transform buildingPrefab2 = null;
    public Transform buildingPrefab3 = null;

	void Start () 
    {
        List<Transform> buildingPrefabs = new List<Transform>();
        AddIfNotNull(buildingPrefabs, buildingPrefab1);
        AddIfNotNull(buildingPrefabs, buildingPrefab2);
        AddIfNotNull(buildingPrefabs, buildingPrefab3);

        int width = 5;
        int height = 5;
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                var buildingPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Count - 1)];

                var basePosition = MathUtil.GetWorldPositionFromGridCoordinate(GetComponent<MeshFilter>(), x + .5f, y + .5f, width, height);

                Transform buildingTransform = (Transform)Instantiate(buildingPrefab);

                var offset = buildingTransform.GetComponent<MeshFilter>().mesh.bounds.extents;
                offset.Scale(Vector3.up);

                basePosition = buildingTransform.InverseTransformPoint(basePosition);
                buildingTransform.Translate(basePosition + offset);

                IndieStudioBehavior indieStudio = buildingTransform.GetComponent<IndieStudioBehavior>();
                if (indieStudio != null)
                {
                    indieStudio.IndieDevCount = Random.Range(1, 20);
                }
            }
        }
    }

    private void AddIfNotNull<T>(List<T> items, T item)
    {
        if (item != null)
        {
            items.Add(item);
        }
    }
	
	void Update () 
    {
	}
}
;