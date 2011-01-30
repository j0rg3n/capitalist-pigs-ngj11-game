using UnityEngine;
using System.Collections;
using Irrelevant.Assets.Scripts.AI;

public class IndieDevBehavior : BillboardBehavior 
{
    public AudioClip mySound;
    public Material Death;
    public Material run;
    public Transform Blood;
    public bool alive;
    
    public Vector3 RunDirection
    {
        get { return runDirection; }
    }

	IndieDevBehavior()
	{
		aiDevGuy = new DevGuy();
		aiDevGuy.indieDevBehaviour = this;
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

		//UnityEngine.Debug.Log(string.Format("worldPos {0} {1} {2}", transform.position.x, transform.position.y, transform.position.z));
		Vector3 localPos = MathUtil.GetLocalPositionFromWorld(GlobalObjects.GetMapMesh(), transform.position);
		localPos.x = -localPos.x;
		localPos.y = 0f;
		localPos.x += 5f;
		localPos.z += 5f;
		localPos.x *= 0.1f;
		localPos.z *= 0.1f;
		//UnityEngine.Debug.Log(string.Format("localPos {0} {1} {2}", localPos.x, localPos.y, localPos.z));
		aiDevGuy.Position = new Vector2(localPos.x, localPos.z);
	}

    public void OnMouseClicked()
    {
        audio.PlayOneShot(mySound);
        GetComponent<SpriteAnimator>().currentFrame = 1;
        GetComponent<SpriteAnimator>().Frames = 1;
        GetComponent<SpriteAnimator>().SetMaterial(Death);
        alive = false;
        GlobalObjects.GetGlobbalGameState().addkillscore();
        if (runDirection.x > 0)
            Instantiate(Blood, transform.localPosition + new Vector3(0.7f, -0.7f, 0), transform.rotation);
        else
            Instantiate(Blood, transform.localPosition + new Vector3(-0.7f, -0.7f, 0), transform.rotation);
        
    }
	
	public override void Update () 
    {
        if (alive)
		{
			Vector3 oldLocalPos = MathUtil.GetLocalPositionFromWorld(GlobalObjects.GetMapMesh(), transform.position);
			Vector3 localPos = new Vector3(aiDevGuy.Position.x, 0, aiDevGuy.Position.y);
			localPos.x *= 10f;
			localPos.z *= 10f;
			localPos.x -= 5f;
			localPos.z -= 5f;
			localPos.y = oldLocalPos.y;
			localPos.x = -localPos.x;
			transform.position = MathUtil.GetWorldPositionFromLocal(GlobalObjects.GetMapMesh(), localPos);
            //var pos = transform.position;
            //pos += runDirection * Time.deltaTime;
            //transform.position = pos;
		}

        base.Update();
	}

    private Vector3 runDirection;

	public DevGuy aiDevGuy; // TODO:m init this
}
