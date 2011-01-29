using UnityEngine;

public class MathUtil
{
    public static Vector3 GetWorldPositionFromGridIndex(MeshFilter planeMeshFilter, int index, int width, int height)
    {
        return GetWorldPositionFromGridCoordinate(planeMeshFilter, index % width, index / width, width, height);
    }

    public static Vector3 GetWorldPositionFromGridCoordinate(MeshFilter planeMeshFilter, int x, int y, int width, int height)
    {
        var localPoint = GetLocalPositionFromGridCoordinate(planeMeshFilter, x, y, width, height);
            
        var transform = planeMeshFilter.GetComponent<Transform>();
        return transform.TransformPoint(localPoint);
    }

    /// <summary>
    /// This assumes that the plane has <c>Vector3D.up</c> as its up axis.
    /// Gets the center of the grid cell.
    /// </summary>
    private static Vector3 GetLocalPositionFromGridCoordinate(MeshFilter planeMeshFilter, int x, int y, int width, int height)
    {
        var localBounds = planeMeshFilter.mesh.bounds;
        var localPoint = new Vector3(localBounds.min.x + (localBounds.max.x - localBounds.min.x) * (float)(x + .5f) / (float)width,
            0f,
            localBounds.min.z + (localBounds.max.z - localBounds.min.z) * (float)(y + .5f) / (float)height);
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

    private MathUtil()
    {
    }
}
