using UnityEngine;
using System.Collections;
using System;

public class PieChartGUIBehavior : MonoBehaviour 
{
    public float size = 128f;

    // TODO: Guessing the anchor from the alignment is confusing. Replace with explicit anchor.
    public Vector2 alignment = new Vector2(.5f, .5f);
    public Texture2D rightBackground;
    public Texture2D right;
    public Texture2D overlay;
    public float amount = 0;
    public bool anchorToScreen;

    void OnGUI()
    {
        var wrappedAmount = amount - Mathf.Floor(amount);

        Vector2 screenPos;
        if (anchorToScreen)
        {
            var counterAlignmentOffset = new Vector2(alignment.x < .5f ? 0 : -size, alignment.y < .5f ? size : 0);
            screenPos = new Vector2(Screen.width, Screen.height);
            screenPos.Scale(alignment);
            screenPos += counterAlignmentOffset;
        }
        else
        {
            screenPos = MathUtil.GetBasePointWithAlignment(gameObject, alignment);

            // TODO: Find out why WorldToScreenPoint gets the Y 
            // coordinate inverted.
            screenPos.y = Screen.height - screenPos.y;
        }

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

        if (overlay != null)
        {
            DrawRotatedTexture(clockRect, overlay, 0);
        }
    }

    private void DrawRotatedTexture(Rect clockRect, Texture2D texture, float amount)
    {
        var center = new Vector2((clockRect.xMin + clockRect.xMax) / 2,
            (clockRect.yMin + clockRect.yMax) / 2);

        Matrix4x4 prevTransform = GUI.matrix;
        GUIUtility.RotateAroundPivot(amount * 360, center);
        
        // This is the manual equivalent of RotateAroundPivot. Gives the same result.
        /*var rot = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(amount * 360, new Vector3(0, 0, 1)), Vector3.one);
        var trans = Matrix4x4.TRS(new Vector3(center.x, center.y, 0), Quaternion.identity, Vector3.one);
        GUI.matrix = trans * rot * trans.inverse;*/
        
        GUI.DrawTexture(clockRect, texture, ScaleMode.StretchToFill, true, 1);
        GUI.matrix = prevTransform;
    }
}
