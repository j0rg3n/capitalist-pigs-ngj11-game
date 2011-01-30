using UnityEngine;

public class RandomUtil
{
    public static Vector3 GetPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.center.x, bounds.center.x + bounds.extents.x);
        float y = Random.Range(bounds.center.y, bounds.center.y + bounds.extents.y);
        float z = Random.Range(bounds.center.z, bounds.center.z + bounds.extents.z);
        return new Vector3(x, y, z);
    }

    private RandomUtil()
    {
    }
}
