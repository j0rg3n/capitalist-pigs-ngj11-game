using UnityEngine;
using System.Collections;
using System;

public class PieChartGUIBehavior : MonoBehaviour 
{
    public float size = 128f;
    public Vector2 alignment = new Vector2(.5f, .5f);
    public Texture2D rightBackground;
    public Texture2D right;
    public float amount = 0;

    void OnGUI()
    {
        var wrappedAmount = amount - Mathf.Floor(amount);

        Vector2 screenPos = GetBasePointWithAlignment();

        // TODO: Find out why WorldToScreenPoint gets the Y 
        // coordinate inverted.
        screenPos.y = Screen.height - screenPos.y;

        var clockRect = new Rect(screenPos.x - (size - size * alignment.x),
            screenPos.y - (size - size * alignment.y),
            size,
            size);
        
        if (wrappedAmount < .5f)
        {
            DrawRotatedTexture(clockRect, right, 0);
            DrawRotatedTexture(clockRect, rightBackground, wrappedAmount);
            DrawRotatedTexture(clockRect, rightBackground, .5f);
        }
        else if (wrappedAmount >= .5f)
        {
            DrawRotatedTexture(clockRect, right, .5f);
            DrawRotatedTexture(clockRect, rightBackground, wrappedAmount);
            DrawRotatedTexture(clockRect, right, 0);
        }
    }

    private Vector2 GetBasePointWithAlignment()
    {
        var camera = GlobalObjects.GetMainCamera();

        Bounds bounds = GetComponent<MeshFilter>().mesh.bounds;

        Vector2 screenBoundsMax = new Vector2(float.MinValue, float.MinValue);
        Vector2 screenBoundsMin = new Vector2(float.MaxValue, float.MaxValue);
        foreach (Vector3 localBoundCorner in MathUtil.GetBoundsCorners(bounds))
        {
            var worldBoundCorner = transform.TransformPoint(localBoundCorner);
            Vector2 screenBoundsCorner = camera.WorldToScreenPoint(worldBoundCorner);

            screenBoundsMin = Vector2.Min(screenBoundsMin, screenBoundsCorner);
            screenBoundsMax = Vector2.Max(screenBoundsMax, screenBoundsCorner);
        }

        Vector2 screenBoundsSize = new Vector2(screenBoundsMax.x - screenBoundsMin.x, screenBoundsMax.y - screenBoundsMin.y);

        Vector2 basePointWithAlignment = screenBoundsSize;
        // TODO: Again with the inverted Y! Why!?
        basePointWithAlignment.Scale(new Vector2(alignment.x, 1 - alignment.y));
        return screenBoundsMin + basePointWithAlignment;

        /*
        Vector2 inverseScreenBoundsSize = screenBoundsSize;
        inverseScreenBoundsSize.x = inverseScreenBoundsSize.x != 0 ? 1 / inverseScreenBoundsSize.x : 1;
        inverseScreenBoundsSize.y = inverseScreenBoundsSize.y != 0 ? 1 / inverseScreenBoundsSize.y : 1;

        float closestDistance = float.MaxValue;
        Vector2 closestCornerPoint = Vector2.zero;
        foreach (Vector3 localBoundCorner in MathUtil.GetBoundsCorners(bounds))
        {
            var worldBoundCorner = transform.TransformPoint(localBoundCorner);
            Vector2 screenBoundsCorner = camera.WorldToScreenPoint(worldBoundCorner);

            // Scale to -1..1 range
            Vector2 scaledScreenBoundsCorner = screenBoundsCorner - screenBoundsMin;
            scaledScreenBoundsCorner.Scale(inverseScreenBoundsSize);

            float distance = (scaledScreenBoundsCorner - alignment).magnitude;

            if (closestDistance < distance)
            {
                closestDistance = distance;
                closestCornerPoint = screenBoundsCorner;
            }
        }

        return closestCornerPoint;
        */
    }

    private static void DrawRotatedTexture(Rect clockRect, Texture2D texture, float amount)
    {
        var center = new Vector2((clockRect.xMin + clockRect.xMax) / 2,
            (clockRect.yMin + clockRect.yMax) / 2);

        Matrix4x4 prevTransform = GUI.matrix;
        GUIUtility.RotateAroundPivot(amount * 360, center);
        GUI.DrawTexture(clockRect, texture, ScaleMode.StretchToFill, true, 0);
        GUI.matrix = prevTransform;
    }
}
