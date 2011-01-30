using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlideRotateBehavior : MonoBehaviour 
{
    public Texture2D slide1;
    public Texture2D slide2;
    public Texture2D slide3;
    public Texture2D slide4;
    public Texture2D slide5;
    public Texture2D slide6;

    public Transform pivot;

    public float veerAmplitude = .25f;
    public float slideSwitchSeconds = 1;

	public void Start()
    {
        AddIfNotNull(slides, slide1);
        AddIfNotNull(slides, slide2);
        AddIfNotNull(slides, slide3);
        AddIfNotNull(slides, slide4);
        AddIfNotNull(slides, slide5);
        AddIfNotNull(slides, slide6);

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

        SetSlideTexture(GetIntSlideIndex(slideIndex));
    }

	public void Update() 
    {
        int prevSlideIndex = GetIntSlideIndex(slideIndex);
        if (slideIndexCurve != null)
        {
            slideIndex = slideIndexCurve.Evaluate(Time.time);
        }

        if (prevSlideIndex != GetIntSlideIndex(slideIndex))
        {
            SetSlideTexture(GetIntSlideIndex(slideIndex));
        }

        Vector3 veer = new Vector3(
            veerCurve[0].Evaluate(Time.time),
            veerCurve[1].Evaluate(Time.time),
            veerCurve[2].Evaluate(Time.time));

        transform.rotation = Quaternion.identity;
        transform.position = pivot.transform.position;
        transform.Rotate(Vector3.right, (slideIndex - Mathf.Floor(slideIndex)) * 360);
        transform.Translate(pivotOffset + veer);
        transform.rotation = transform.rotation * pivotRotation;
    }

    public void SetSlide(float targetSlideIndex)
    {
        var now = Time.time;
        slideIndexCurve = AnimationCurve.EaseInOut(now, slideIndex, now + slideSwitchSeconds, targetSlideIndex);
    }

    private void SetSlideTexture(int index)
    {
        Texture2D slide = index >= 0 && index < slides.Count ? slides[index] : null;
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
    private float slideIndex = -1;
    private readonly List<Texture2D> slides = new List<Texture2D>();
}
