using UnityEngine;
using System.Collections;

public class TestSpawnBuildingsOnPlane : MonoBehaviour 
{
    public Transform buildingPrefab;

	void Start () 
    {
        int width = 5;
        int height = 5;
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                var basePosition = MathUtil.GetWorldPositionFromGridCoordinate(GetComponent<MeshFilter>(), x + .5f, y + .5f, width, height);

                Transform buildingTransform = (Transform)Instantiate(buildingPrefab);

                var offset = buildingTransform.GetComponent<MeshFilter>().mesh.bounds.extents;
                offset.Scale(Vector3.up);

                basePosition = buildingTransform.InverseTransformPoint(basePosition);
                buildingTransform.Translate(basePosition + offset);
            }
        }
    }
	
	void Update () 
    {
	}
}
;