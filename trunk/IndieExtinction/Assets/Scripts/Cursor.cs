using UnityEngine;
using System.Collections;

public class Cursor : BillboardBehavior
{
    //public AudioClip mySound;
    public Material cusorMat;
    //public Transform Blood;

    public override void Start()
    {
        base.Start();
        GetComponent<SpriteAnimator>().SetMaterial(cusorMat);
        //var rotation = Quaternion.AngleAxis(angle, Vector3.up);
        //var directionTransform = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);

    }

    public override void Update()
    {
        Vector3 pos = Input.mousePosition;
        pos.z = 10;
        //transform.position = pos;

        base.Update();
    }
}
