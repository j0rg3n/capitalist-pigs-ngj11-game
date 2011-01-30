using UnityEngine;
using System.Collections.Generic;

public class MathUtil
{
    public static IEnumerable<Vector3> GetBoundsCorners(Bounds bounds)
    {
        foreach (float x in new[]{bounds.min.x, bounds.max.x})
        {
            foreach (float y in new[]{bounds.min.y, bounds.max.y})
            {
                foreach (float z in new[]{bounds.min.z, bounds.max.z})
                {
                    yield return new Vector3(x, y, z);
                }
            }
        }
    }

    public static Vector3 GetWorldPositionFromGridCoordinate(MeshFilter planeMeshFilter, float x, float y, int width, int height)
    {
        var localPoint = GetLocalPositionFromGridCoordinate(planeMeshFilter, x, y, width, height);
            
        var transform = planeMeshFilter.GetComponent<Transform>();
        return transform.TransformPoint(localPoint);
    }

	public static Vector3 GetWorldPositionFromLocal(MeshFilter planeMeshFilter, Vector3 localPoint)
	{
		var transform = planeMeshFilter.GetComponent<Transform>();
		return transform.TransformPoint(localPoint);
	}

	public static Vector3 GetLocalPositionFromWorld(MeshFilter planeMeshFilter, Vector3 worldPoint)
	{
		var transform = planeMeshFilter.GetComponent<Transform>();
		return transform.InverseTransformPoint(worldPoint);
	}

	/// <summary>
    /// This assumes that the plane has <c>Vector3D.up</c> as its up axis.
    /// Gets the center of the grid cell.
    /// </summary>
    private static Vector3 GetLocalPositionFromGridCoordinate(MeshFilter planeMeshFilter, float x, float y, int width, int height)
    {
        var localBounds = planeMeshFilter.mesh.bounds;
        var localPoint = new Vector3(localBounds.min.x + (localBounds.max.x - localBounds.min.x) * (width - x) / width,
            0f,
            localBounds.min.z + (localBounds.max.z - localBounds.min.z) * (y) / height);
        return localPoint;
    }

    /*
    public static Rect GetGridCoordinatesFromMeshBounds(MeshFilter planeMeshFilter, MeshFilter projectedObjectMeshFilter, int x, int y)
    {
        var projectedObjectLocalBounds = projectedObjectMeshFilter.mesh.bounds;

        var projectObjectTransform = projectedObjectMeshFilter.GetComponent<Transform>();
        var planeTransform = planeMeshFilter.GetComponent<Transform>();

        // TODO: Transform plane into projected object bounds, so the axis-aligned box becomes a rectangle.
    }
    */

	public static Vector2 GetLocalPositionFromWorldCorrected(MeshFilter planeMeshFilter, Vector3 worldPoint)
	{
		Vector3 localPos = MathUtil.GetLocalPositionFromWorld(GlobalObjects.GetMapMesh(), worldPoint);
		localPos.x = -localPos.x;
		localPos.y = 0f;
		localPos.x += 5f;
		localPos.z += 5f;
		localPos.x *= 0.1f;
		localPos.z *= 0.1f;
		//UnityEngine.Debug.Log(string.Format("localPos {0} {1} {2}", localPos.x, localPos.y, localPos.z));
		return new Vector2(localPos.x, localPos.z);
	}



    private MathUtil()
    {
    }
}
