using UnityEngine;
using System.Collections.Generic;
using Irrelevant.Assets.Scripts;

public class GlobalObjects
{

	public static List<IndieHouseLocation> indieHouseLocations = new List<IndieHouseLocation>();


    public static Camera GetMainCamera()
    {
        return GameObject.Find(MAIN_CAMERA).GetComponent<Camera>(); // GameObject.Find(ObjectNames.MAIN_CAMERA).GetComponent<Camera>();
    }

	public static IndieDevBehavior[] GetIndieDevs()
	{
		return (IndieDevBehavior[])Resources.FindObjectsOfTypeAll(typeof(IndieDevBehavior));
	}

	public static GlobalDevAIBehaviour GetDevAIBehaviour()
	{
		return (GlobalDevAIBehaviour)Object.FindObjectOfType(typeof(GlobalDevAIBehaviour));
	}

	// Marks the class as 'static'. 
    public static GlobalGameStateBehavior GetGlobbalGameState()
    {
        return(GlobalGameStateBehavior)Object.FindObjectOfType(typeof(GlobalGameStateBehavior));
    }

    // Marks the class as 'static'. 
    // Mono doesn't allow the 'static' modifier on classes.
    private GlobalObjects()
    {
    }

    private const string MAIN_CAMERA = "Main Camera";
}
