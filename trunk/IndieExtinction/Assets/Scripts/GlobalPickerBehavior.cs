using UnityEngine;
using System.Collections;

public class GlobalPickerBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (hit)
        {
            if (mouseDown)
            {
                hitInfo.transform.gameObject.SendMessage("OnMouseDown", hitInfo, SendMessageOptions.DontRequireReceiver);
            }

            if (mouseClick)
            {
                hitInfo.transform.gameObject.SendMessage("OnMouseClicked", hitInfo, SendMessageOptions.DontRequireReceiver);
                print(hitInfo.transform.name + " clicked at " + hitInfo.point);
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
            var cam = GameObject.Find(ObjectNames.MAIN_CAMERA).GetComponent<Camera>();

            var mousePos = Event.current.mousePosition;

            // TODO: Find out why ScreenPointToRay expects the Y 
            // coordinate to be inverted.
            mousePos.y = Screen.height - mousePos.y;

            var mouseRay = cam.ScreenPointToRay(mousePos);

            hit = Physics.Raycast(mouseRay, out hitInfo);
        }
    }

    private bool hit;
    private RaycastHit hitInfo;
    private bool mouseDown;
    private bool mouseClick;
}
