using UnityEngine;
using System.Collections;

public class SpriteAnimator : MonoBehaviour {

    public int Frames = 4;
    public float CurrentTime = 0.0f;
    public float UpdateTime = 0.2f;
    public int currentFrame = 1;
    // public Texture2D MainTexture;
    public Vector2 Rundirection;

    public void SetMaterial(Material myMaterial)
    {
        if (renderer.material != myMaterial)
            renderer.material = myMaterial;
    }

    
    
	// Use this for initialization
	void Start () 
    {
        
	}

	// Update is called once per frame
	void Update () 
    {
        CurrentTime += Time.deltaTime;

        if (CurrentTime > UpdateTime)
        {
            CurrentTime = 0.0f;

            Vector2 Vect;
            Rundirection = GetRunDirectionOnScreen();
            if (Rundirection.x >= 0.0f)
            {
                Vect = new Vector2((1.0f / (float)Frames), 1.0f);
            }
            else
            {
                Vect = new Vector2(-(1.0f / (float)Frames), 1.0f);
            }
            renderer.material.SetTextureScale("_MainTex", Vect);

            Vector2 val = renderer.material.mainTextureOffset;
            val.x += (float)(1.0f / Frames);
            renderer.material.mainTextureOffset = val;
            currentFrame += 1;
            if (currentFrame > Frames)
            {
                val = renderer.material.mainTextureOffset;
                val.x = 0.0f;
                renderer.material.mainTextureOffset = val;
                currentFrame = 1;

            }
        }

	}


    private Vector3 GetRunDirectionOnScreen()
    {
        Camera camera = GlobalObjects.GetMainCamera();
        var a = transform.position + GetComponent<IndieDevBehavior>().RunDirection;
        var b = transform.position;
        var aOnScreen = camera.WorldToScreenPoint(a);
        var bOnScreen = camera.WorldToScreenPoint(b);
        return aOnScreen - bOnScreen;
    }
}
