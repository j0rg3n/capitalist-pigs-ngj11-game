using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct IndexedHit
{
    public RaycastHit hit;
    public int index;
}

public sealed class BigHitInfo
{
    public IndexedHit pointHit;
    public IndexedHit sphereHit;
}

public class GlobalPickerBehavior : MonoBehaviour 
{
    public const int SPHERE_CAST_RADIUS = 2;

	// Use this for initialization
	void Start () 
    {	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (pointHits != null && pointHits.Length > 0)
        {
            for (int i = 0; i < pointHits.Length; ++i)
            {
                IndexedHit hitInfo = new IndexedHit() { index = i, hit = pointHits[i] };
                if (mouseDown)
                {
                    hitInfo.hit.transform.gameObject.SendMessage("OnMouseDown", hitInfo, SendMessageOptions.DontRequireReceiver);
                }

                if (mouseClick)
                {
                    hitInfo.hit.transform.gameObject.SendMessage("OnMouseClicked", hitInfo, SendMessageOptions.DontRequireReceiver);
                    print(hitInfo.hit.transform.name + " clicked at " + hitInfo.hit.point);
                }
            }
        }

        if (sphereHits != null && sphereHits.Length > 0)
        {
            for (int i = 0; i < sphereHits.Length; ++i)
            {
                IndexedHit hitInfo = new IndexedHit() { index = i, hit = sphereHits[i] };
                if (mouseDown)
                {
                    hitInfo.hit.transform.gameObject.SendMessage("OnBigMouseDown", hitInfo, SendMessageOptions.DontRequireReceiver);
                }

                if (mouseClick)
                {
                    hitInfo.hit.transform.gameObject.SendMessage("OnBigMouseClicked", hitInfo, SendMessageOptions.DontRequireReceiver);
                    print(hitInfo.hit.transform.name + " big-clicked at " + hitInfo.hit.point);
                }
            }
        }

        mouseClick = false;
    }

    void OnGUI()
    {
        switch (Event.current.type)
        {
            case EventType.MouseDown:
                mouseDown = true;
                break;
            case EventType.MouseUp:
                mouseDown = false;
                mouseClick = true;
                break;
        }

        if (Event.current.isMouse)
        {
            var cam = GlobalObjects.GetMainCamera();

            var mousePos = Event.current.mousePosition;

            // TODO: Find out why ScreenPointToRay expects the Y 
            // coordinate to be inverted.
            mousePos.y = Screen.height - mousePos.y;

            var mouseRay = cam.ScreenPointToRay(mousePos);


            pointHits = Physics.RaycastAll(mouseRay);
            sphereHits = Physics.SphereCastAll(mouseRay, SPHERE_CAST_RADIUS);
        }
    }

    private bool hit;
    private RaycastHit[] pointHits;
    private RaycastHit[] sphereHits;
    private bool mouseDown;
    private bool mouseClick;
}
