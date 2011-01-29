using UnityEngine;

public class RandomUtil
{
    public static Vector3 GetPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.center.x, bounds.center.x + bounds.extents.x);
        float y = Random.Range(bounds.center.x, bounds.center.x + bounds.extents.x);
        float z = Random.Range(bounds.center.x, bounds.center.x + bounds.extents.x);
        return new Vector3(x, y, z);
    }

    private RandomUtil()
    {
    }
}
