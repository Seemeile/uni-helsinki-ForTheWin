using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 40f;
    public float borderThickness = 25f;

    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.mousePosition.y >= Screen.height - borderThickness)
        {
            pos.y += speed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= borderThickness)
        {
            pos.y -= speed * Time.deltaTime;
        }
        if (Input.mousePosition.x >= Screen.width - borderThickness)
        {
            pos.x += speed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= borderThickness)
        {
            pos.x -= speed * Time.deltaTime;
        }

        transform.position = pos;
    }
}
