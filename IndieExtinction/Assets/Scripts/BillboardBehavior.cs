using UnityEngine;
using System.Collections;

/// <summary>
/// Makes the given direction on the object point towards the screen.
/// </summary>
public class BillboardBehavior : MonoBehaviour 
{
    public Vector3 objectFrontVector = Vector3.up;
    public float roll = 0;

	// Use this for initialization
	public virtual void Start () 
    {
        objectFrontVector.Normalize();   
	}
	
	// Update is called once per frame
    public virtual void Update() 
    {
        var cam = GlobalObjects.GetMainCamera();

        Vector3 toScreenVector = cam.transform.TransformDirection(Vector3.back);

        var rollRotation = Quaternion.AngleAxis(roll, objectFrontVector);

        var toScreenRotation = Quaternion.FromToRotation(objectFrontVector, toScreenVector);

        transform.rotation = toScreenRotation * rollRotation;
    }
}