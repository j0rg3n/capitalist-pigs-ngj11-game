using UnityEngine;
using System.Collections;

public class IndieDevBehavior : BillboardBehavior 
{
    public AudioClip mySound;
    public Material Death;
    public Material run;
    private bool alive;
    
    public Vector3 RunDirection
    {
        get { return runDirection; }
    }

	public override void Start () 
    {
        base.Start();
        GetComponent<SpriteAnimator>().SetMaterial(run);
        alive = true;
        float angle = Random.Range(0f, 360f);
        var rotation = Quaternion.AngleAxis(angle, Vector3.up);
        var directionTransform = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
        runDirection = directionTransform.MultiplyVector(Vector3.forward);
	}

    public void OnMouseClicked()
    {
        audio.PlayOneShot(mySound);
        GetComponent<SpriteAnimator>().currentFrame = 1;
        GetComponent<SpriteAnimator>().Frames = 1;
        GetComponent<SpriteAnimator>().SetMaterial(Death);
        alive = false;
        GlobalObjects.GetGlobbalGameState().addscore();
    }
	
	public override void Update () 
    {
        if (alive)
		{
            var pos = transform.position;
            pos += runDirection * Time.deltaTime;
            transform.position = pos;
		}

        base.Update();
	}

    private Vector3 runDirection;
}
