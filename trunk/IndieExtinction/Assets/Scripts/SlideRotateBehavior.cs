using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlideRotateBehavior : MonoBehaviour 
{
    public Texture2D[] slides;

    public Transform pivot;

    public float veerAmplitude = .25f;
    public float slideSwitchSeconds = 1;

	public void Start()
    {
        pivotOffset = transform.position - pivot.transform.position;
        pivotRotation = transform.rotation;
        pivot.GetComponent<MeshRenderer>().enabled = false;

        for (int j = 0; j < 3; ++j)
        {
            var curve = new AnimationCurve();
            curve.preWrapMode = WrapMode.Loop;
            curve.postWrapMode = WrapMode.Loop;
            
            float t = 0;
            for (int i = 0; i < 20; ++i)
            {
                curve.AddKey(i, Random.Range(-.1f, .1f) * veerAmplitude);
                t += Random.Range(.25f, 3f);
            }
            veerCurve[j] = curve;
        }

        SetSlideTexture(GetIntSlideIndex(prevSlideIndex));
    }

	public void Update() 
    {
        float t = Time.time;

        float slideRotation = .5f;
        if (slideIndexCurve != null)
        {
            float slideIndex = slideIndexCurve.Evaluate(t);
            slideRotation = slideIndex - Mathf.Floor(slideIndex);
            SetSlideTexture(GetIntSlideIndex(slideIndex));
        }

        var zoomOffset = Vector3.zero;
        if (zoomCurve != null)
        {
            zoomOffset = Vector3.forward * zoomCurve.Evaluate(t);
        }

        var veerOffset = new Vector3(
            veerCurve[0].Evaluate(Time.time),
            veerCurve[1].Evaluate(Time.time),
            veerCurve[2].Evaluate(Time.time));

        transform.rotation = Quaternion.identity;
        transform.position = pivot.transform.position;
        transform.Rotate(Vector3.right, slideRotation * 360);
        transform.Translate(pivotOffset + veerOffset + zoomOffset);
        transform.rotation = transform.rotation * pivotRotation;
    }

    public void SetSlide(float targetSlideIndex, float targetZoom)
    {
        var now = Time.time;
        slideIndexCurve = AnimationCurve.EaseInOut(now, prevSlideIndex, now + slideSwitchSeconds, targetSlideIndex);
        prevSlideIndex = targetSlideIndex;
        zoomCurve = AnimationCurve.EaseInOut(now, prevZoom, now + slideSwitchSeconds, targetZoom);
        prevZoom = targetZoom;
    }

    private void SetSlideTexture(int index)
    {
        Texture2D slide = index >= 0 && index < slides.Length ? slides[index] : null;
        GetComponent<MeshRenderer>().material.mainTexture = slide;
        GetComponent<MeshRenderer>().enabled = slide != null;
    }

    private static void AddIfNotNull<T>(List<T> items, T item)
    {
        if (item != null)
        {
            items.Add(item);
        }
    }

    private static int GetIntSlideIndex(float slideIndex)
    {
        return (int)(slideIndex + 1.5f) - 1;
    }

    private readonly AnimationCurve[] veerCurve = new AnimationCurve[3];
    private Vector3 pivotOffset;
    private Quaternion pivotRotation;
    private AnimationCurve slideIndexCurve;
    private AnimationCurve zoomCurve;
    private float prevZoom = 0;
    private float prevSlideIndex = -1;
}