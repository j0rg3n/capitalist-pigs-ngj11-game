using UnityEngine;
using System.Collections;
using Irrelevant.Assets.Scripts.AI;

public class IndieDevBehavior : BillboardBehavior
{
    public AudioClip[] Splats;
    public AudioClip[] Screams;
    public AudioClip[] Laughs;
    public Material Death;
    public Material run;
    public Transform Blood;
    public bool alive;
    public float fadetime = 5;
    public float times;

    private Color alfa;
    
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
        times = 0.0f;
        GetComponent<SpriteAnimator>().SetMaterial(run);
        alive = true;
		runDirection = new Vector3(0f, 0f, 0f);

		aiDevGuy.Position = MathUtil.GetLocalPositionFromWorldCorrected(GlobalObjects.GetMapMesh(), transform.position);
	}

    public void OnMouseClicked()
    {
        PlayDeathSounds();
        GetComponent<SpriteAnimator>().currentFrame = 1;
        GetComponent<SpriteAnimator>().Frames = 1;
        GetComponent<SpriteAnimator>().SetMaterial(Death);
        alive = false;
        //transform.collider.active = false;
        GlobalObjects.GetGlobbalGameState().addkillscore();
        if (runDirection.x > 0)
            Instantiate(Blood, transform.localPosition + new Vector3(0.7f, -0.7f, 0), transform.rotation);
        else
            Instantiate(Blood, transform.localPosition + new Vector3(-0.7f, -0.7f, 0), transform.rotation);
        
    }

	public Vector3 GetAIWorldTransform()
	{
		Vector3 oldLocalPos = MathUtil.GetLocalPositionFromWorld(GlobalObjects.GetMapMesh(), transform.position);
		Vector3 localPos = new Vector3(aiDevGuy.Position.x, 0, aiDevGuy.Position.y);
		localPos.x *= 10f;
		localPos.z *= 10f;
		localPos.x -= 5f;
		localPos.z -= 5f;
		localPos.y = oldLocalPos.y;
		localPos.x = -localPos.x;
		Vector3 newWS = transform.TransformPoint(localPos);
		Vector3 newSS = GlobalObjects.GetMainCamera().WorldToScreenPoint(newWS);
		Vector3 oldSS = GlobalObjects.GetMainCamera().WorldToScreenPoint(transform.position);
		runDirection = oldSS - newSS;
		return MathUtil.GetWorldPositionFromLocal(GlobalObjects.GetMapMesh(), localPos);
	}
	
	public override void Update () 
    {
        if (alive)
        {
            transform.position = GetAIWorldTransform();
        }
        else
        {
            times += Time.deltaTime;
            alfa = transform.renderer.material.color;
            alfa.a -= alfa.a * ( (times * 0.05f)/ fadetime);
            transform.renderer.material.color = alfa;
            if (times > fadetime)
                Destroy(gameObject);
        }

        base.Update();
	}

    private void PlayDeathSounds()
    {
        if (Splats.Length > 0)
        {
            AudioClip splat = Splats[Random.Range(0, Splats.Length)];
            if (splat != null)
            {
                audio.PlayOneShot(splat);
            }
        }
        if (Screams.Length > 0)
        {
            AudioClip scream = Screams[Random.Range(0, Screams.Length)];
            if (scream != null)
            {
                audio.PlayOneShot(scream);
            }
        }
        if (Laughs.Length > 0)
        {
            if (Random.value < 0.10)
            {
                AudioClip laugh = Laughs[Random.Range(0, Laughs.Length)];
                if (laugh != null)
                {
                    audio.PlayOneShot(laugh);
                }
            }
        }
    }

    private Vector3 runDirection;

	public DevGuy aiDevGuy;
}
