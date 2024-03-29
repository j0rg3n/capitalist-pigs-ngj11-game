using UnityEngine;
using System.Collections.Generic;
using Irrelevant.Assets.Scripts;

public class GlobalObjects
{

	public static List<IndieHouseLocation> indieHouseLocations = new List<IndieHouseLocation>();


	public static Camera GetMainCamera()
	{
        var cam = GameObject.Find(MAIN_CAMERA);
        if (cam == null)
        {
            // HACK: Check for misspelled name, too.
            cam = GameObject.Find("main Camera");
        }
        return cam.GetComponent<Camera>();
    }

	public static MeshFilter GetMapMesh()
	{
		return GameObject.Find(MAP).GetComponent<MeshFilter>();
	}

    public static PieChartGUIBehavior GetHealthPie()
    {
        return GameObject.Find(HEALTH_PIE).GetComponent<PieChartGUIBehavior>();
    }

    public static guiScriptBehavior GetGUIScriptBehavior()
    {
        return FindObjectOfType<guiScriptBehavior>();
    }

	public static IndieDevBehavior[] GetIndieDevs()
	{
		return (IndieDevBehavior[])Resources.FindObjectsOfTypeAll(typeof(IndieDevBehavior));
	}

    public static StudioBehavior GetStudio()
    {
        return FindObjectOfType<StudioBehavior>();
    }

	public static GlobalDevAIBehaviour GetDevAIBehaviour()
	{
		return FindObjectOfType<GlobalDevAIBehaviour>();
	}

	// Marks the class as 'static'. 
    public static GlobalGameStateBehavior GetGlobbalGameState()
    {
        return FindObjectOfType<GlobalGameStateBehavior>();
    }

    public static SlideRotateBehavior GetSlideProjector()
    {
        return FindObjectOfType<SlideRotateBehavior>();
    }

    // Marks the class as 'static'. 
    // Mono doesn't allow the 'static' modifier on classes.
    private GlobalObjects()
    {
    }

	private const string MAIN_CAMERA = "Main Camera";
    private const string MAP = "Map";
    private const string HEALTH_PIE = "HealthPie";

    private static T FindObjectOfType<T>() where T : Object
    {
        return (T)Object.FindObjectOfType(typeof(T));
    }

}
