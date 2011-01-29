using UnityEngine;
using System.Collections;

/// <summary>
/// Makes the given direction on the object point towards the screen.
/// </summary>
public class BillboardBehavior : MonoBehaviour 
{
    public Vector3 objectFrontVector = Vector3.up;

	// Use this for initialization
	void Start () 
    {
        objectFrontVector.Normalize();   
	}
	
	// Update is called once per frame
	void Update () 
    {
        var cam = GetMainCamera();

        Vector3 toScreenVector = cam.transform.TransformDirection(Vector3.back);

        var toScreenRotation = new Quaternion();
        toScreenRotation.SetFromToRotation(objectFrontVector, toScreenVector);

        transform.rotation = toScreenRotation;
    }

    private static Camera GetMainCamera()
    {
        return GameObject.Find(ObjectNames.MAIN_CAMERA).GetComponent<Camera>();
    }
}