using UnityEngine;
using System.Collections;

public class BillboardBehavior : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update () 
    {
        // This doesn't work. Do the math, please.
        var cam = GetComponent<Camera>();
        transform.rotation.SetLookRotation(cam.transform.position);
    }
}