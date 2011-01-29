using UnityEngine;
using System.Collections;

public class SpriteAnimator : MonoBehaviour {

    public int Frames = 4;
    
    int currentFrame = 1;
    public float CurrentTime = 0.0f;
    public float UpdateTime = 0.2f;
    


	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        CurrentTime += Time.deltaTime;

        if (CurrentTime > UpdateTime)
        {
            CurrentTime = 0.0f;
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
}
