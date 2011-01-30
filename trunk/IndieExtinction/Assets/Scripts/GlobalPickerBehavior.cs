using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct IndexedHit
{
    public RaycastHit hit;
    public int index;
}

public class GlobalPickerBehavior : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {	
	}
	
	// Update is called once per frame
	void Update () 
    {
        var globalGameState = GetComponent<GlobalGameStateBehavior>();
        if (globalGameState.GameOver)
        {
            if (mouseClick && globalGameState.SecondsSinceGameOver > 3.5f)
            {
                globalGameState.LoadIntroLevel();
                return; 
            }
        }
        else
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
                        //print(hitInfo.hit.transform.name + " clicked at " + hitInfo.hit.point);
                    }
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
        }
    }

    private bool hit;
    private RaycastHit[] pointHits;
    private bool mouseDown;
    private bool mouseClick;
}
