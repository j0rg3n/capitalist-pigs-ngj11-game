using UnityEngine;
using System.Collections;

public class IndieDevBehavior : MonoBehaviour 
{
    public AudioClip mySound;

    public Vector3 RunDirection
    {
        get { return runDirection; }
    }

	void Start () 
    {	
        float angle = Random.Range(0f, 360f);
        var rotation = Quaternion.AngleAxis(angle, Vector3.up);
        var directionTransform = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
        runDirection = directionTransform.MultiplyVector(Vector3.forward);
	}

    public void OnMouseClicked()
    {
        audio.PlayOneShot(mySound);
    }
	
	void Update () 
    {
        transform.Translate(runDirection * Time.deltaTime);
	}

    private Vector3 runDirection;
}
