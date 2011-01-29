using UnityEngine;
using System.Collections;

public class PieChartGUIBehavior : MonoBehaviour 
{
    public float size = 128f;
    public Vector2 alignment = new Vector2(.5f, .5f);
    public Texture2D rightBackground;
    public Texture2D right;
    public float amount = 0;

    void OnGUI()
    {
        amount += Time.deltaTime * .25f;

        var wrappedAmount = amount - Mathf.Floor(amount);

        var camera = GlobalObjects.GetMainCamera();
        var screenPos = camera.WorldToScreenPoint(transform.position);

        var clockRect = new Rect(screenPos.x - (size - size * alignment.x),
            screenPos.y - (size - size * alignment.y),
            size * alignment.x,
            size * alignment.y);
        
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
