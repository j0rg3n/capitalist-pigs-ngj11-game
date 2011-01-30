using UnityEngine;
using System.Collections;

public class CameraColor : MonoBehaviour {

    Color redish = Color.red;
    Color Blueish = Color.blue;
    float duration = 4.0f;
	// Use this for initialization
	void Start () {
        camera.clearFlags = CameraClearFlags.SolidColor;
	}
	
	// Update is called once per frame
	void Update () {
	float t = Mathf.PingPong(Time.time, duration)/duration;
    camera.backgroundColor = Color.Lerp(redish, Blueish, t);
	}
}
