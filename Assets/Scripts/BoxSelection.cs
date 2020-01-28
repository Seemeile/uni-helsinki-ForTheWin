using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BoxSelection : MonoBehaviour
{
    public GUIStyle MouseDragSkin;
    private bool UserIsDragging = false;
    public Vector3 mouseDownPosition;
    public Vector3 currentMousePosition;

    

    void Update()
    {
        currentMousePosition.x = Input.mousePosition.x;
        currentMousePosition.y = Input.mousePosition.y;

        if (Input.GetMouseButtonDown(0))
           {
           UserIsDragging = true;
           mouseDownPosition.x = Input.mousePosition.x;
           mouseDownPosition.y = Input.mousePosition.y;
        }
        if (Input.GetMouseButtonUp(0))
           {
            UserIsDragging = false;
           }
    }

    void OnGUI()
    {
        if (UserIsDragging)
        {
            float boxWidth = mouseDownPosition.x - currentMousePosition.x;
            float boxHeight = mouseDownPosition.y - currentMousePosition.y;
            float boxBottom = currentMousePosition.x;
            float boxTop = Screen.height - currentMousePosition.y - boxHeight;
            GUI.Box(new Rect(boxBottom, boxTop, boxWidth, boxHeight), "", MouseDragSkin);
        }
    }
}
