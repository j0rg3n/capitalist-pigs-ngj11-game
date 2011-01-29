using UnityEngine;
using System.Collections;

public class GlobalGameStateBehavior : MonoBehaviour
{
    public int score = 100;
    public int kill = 10;

    public void addscore()
    {
        score += kill;
    }

    public int getscore
    {
        get { return score; }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
